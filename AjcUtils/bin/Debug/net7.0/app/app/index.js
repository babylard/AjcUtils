"use strict";

const {app, BrowserWindow, clipboard, dialog, ipcMain, Menu, shell} = require("electron");
const {autoUpdater} = require("electron-updater");
const crypto = require("crypto");
const path = require("path");
const Store = require("electron-store");
const {machineId} = require("node-machine-id");
const {v4: uuidv4} = require('uuid');
const os = require("os");
const config = require("./config.js");
const server = require("./server.js");
const translation = require("./translation.js");

const AUTO_UPDATE_STARTUP_DELAY_MS = 2000;
const AUTO_UPDATE_PERIODIC_DELAY_MS = 1 * 60 * 60 * 1000; 

let win = null;

let printWindow = null;

const store = new Store();

const log = (level, message) => {
  if (win) {
    if (typeof message === "object") {
      message = message.stack || message.error.stack;
    }
    if (["debug", "debugError"].includes(level) && config.showTools) {
      win.webContents.send("log", {level: level === "debug" ? "debug" : "error", message});
    }
    else {
      if (["info", "warn", "error"].includes(level)) {
        win.webContents.send("log", {level, message});
      }
    }
  }
  else {
    setTimeout(() => {
      log(level, message);
    }, 1000);
  }
};

process.on("uncaughtException", err => {
  log("error", `Uncaught exception: ${err.stack || err.error.stack}`);
  setTimeout(() => {
    process.exit(1);
  }, 100);
});

process.on("unhandledRejection", err => {
  log("error", `Unhandled rejection: ${err.stack || err.error.stack}`);
});

let rcToken = "";
for (let i = 0; i < process.argv.length; i++) {
  if (process.argv[i] == "--rc-token") {
    if (process.argv[++i]) {
      rcToken = crypto.createHash("sha1").update(process.argv[i]).digest("hex");
      break;
    }
  }
}

const pack = require("./package.json");

// clear local save data
if (config.clearStorage) {
  store.clear();
}

let webview = null; // ref to webview within index.html

const setApplicationMenu = () => {
  log("info", "Enabling Dev Menu.");
  Menu.setApplicationMenu(Menu.buildFromTemplate([{
    label: "Development",
    submenu: [{
      label: "Reload",
      accelerator: "CmdOrCtrl+R",
      click: () => {
        win.webContents.reloadIgnoringCache();
      },
    }, {
      label: "Toggle DevTools",
      accelerator: "CmdOrCtrl+Shift+I",
      click: () => {
        win.toggleDevTools();
        win.webContents.send("toggleDevTools");
      },
    }],
  }]));
};

const loadClient = () => {
  win.loadURL(`file://${__dirname}/gui/index.html`);
};

const updateStatus = {
  state: "idle",
  progress: 0,
};

let autoUpdateTimeoutId = null;

const scheduleAutoUpdate = (delayMs) => {
  log("debug", `Scheduled update check: ${delayMs}ms`);
  if (autoUpdateTimeoutId !== null) {
    clearTimeout(autoUpdateTimeoutId);
  }
  autoUpdateTimeoutId = setTimeout(() => autoUpdater.checkForUpdates(), delayMs);
};

const autoUpdateProgress = (state, progress = null) => {
  updateStatus.state = state;
  updateStatus.progress = progress || null;
  win.webContents.send("autoUpdateStatus", updateStatus);
};

if (!config.noUpdater) {
  scheduleAutoUpdate(AUTO_UPDATE_STARTUP_DELAY_MS);
  autoUpdater.on("error", (error) => {
    log("error", `Error with auto update! url:${config.update} - ${error}`);
    autoUpdateProgress("error");
    scheduleAutoUpdate(AUTO_UPDATE_PERIODIC_DELAY_MS);
  });
  autoUpdater.on("checking-for-update", () => {
    log("info", "Checking for update...");
    autoUpdateProgress("check");
  });
  autoUpdater.on("update-available", () => {
    log("info", "Update available...");
    autoUpdateProgress("download", 1);
  });
  autoUpdater.on("update-not-available", () => {
    log("info", "Update not available...");
    autoUpdateProgress("idle");
    scheduleAutoUpdate(AUTO_UPDATE_PERIODIC_DELAY_MS);
  });
  autoUpdater.on("download-progress", (progress) => {
    log("info", `Download progress: ${JSON.stringify(progress)}`);
    autoUpdateProgress("download", progress.percent);
  });
  autoUpdater.on("update-downloaded", () => {
    log("info", "Update downloaded, will install on next restart.");
    autoUpdateProgress("restart");
    store.set("app.lastUpdatedAt", new Date().getTime());
  });
}

// communication with renderer
ipcMain.on("loaded", async (event, message) => {
  webview = event.sender;
  const username = store.get("login.username") || "";
  // Default remember me to true
  const rememberMe = store.get("login.rememberMe") !== false;
  const authToken = store.get("login.authToken") || null;
  const refreshToken = store.get("login.refreshToken") || null;
  const df = await getDf();

  webview.send("loginInfoLoaded", {
    username,
    authToken,
    refreshToken,
    rememberMe,
    df,
    config,
    rcToken,
  });

  if (Object.keys(store.store).length === 0) {
    log("debug", "Listening for Autologin data.");
    (async () => {
      try {
        const data = await server.listenForAutoLogin();
        log("debug", `Webserver stopped, ${data ? "received data." : "did not receive data."}`);
        if (data) {
          if (data.affiliateCode) {
            store.set("login.affiliateCode", data.affiliateCode);
          }
          win.webContents.send("obtainedToken", {
            token: data.authToken,
          });
        }
      }
      catch (err) {
        log("debugError", JSON.stringify(err));
      }
    })();
  }
  // TODO: Optimize this, when returning to maximized from full screen it fires of a leave-full-screen event and triggers an extra  disk write with the "windowed" state

  // Maximize event fires after the full screen event
  win.on("enter-full-screen", () => {
    setTimeout(() => {
      // win.webContents.send("log", "fullScreen");
      store.set("window.state", "fullScreen");
      webview.send("screenChange", "fullScreen");
    }, 1);
  });

  win.on("maximize", () => {
    // win.webContents.send("log", "maximized");
    store.set("window.state", "maximized");
    webview.send("screenChange", "maximized");
  });

  win.on("unmaximize", () => {
    // win.webContents.send("log", "windowed");
    store.set("window.state", "windowed");
    webview.send("screenChange", "windowed");
  });

  win.on("leave-full-screen", () => {
    // win.webContents.send("log", "windowed");
    store.set("window.state", "windowed");
    webview.send("screenChange", "windowed");
  });

  win.on("close", () => {
    const position = win.getPosition();
    store.set("window.x", position[0]);
    store.set("window.y", position[1]);
  });
});

ipcMain.on("loginSucceeded", (event, message) => {
  log("debug", `Saving login data: ${JSON.stringify(message)}`);
  server.stop();

  store.set("login.username", message.username);
  store.set("login.language", message.language);
  translation.setLanguage(message.language);
  store.set("login.rememberMe", message.rememberMe);
  if (message.rememberMe) {
    store.set("login.authToken", message.authToken);
    if (message.refreshToken) {
      store.set("login.refreshToken", message.refreshToken);
    }
  }
  else {
    store.delete("login.authToken");
    store.delete("login.refreshToken");
  }

  if (message.accountType > 2) {
    setApplicationMenu();
  }
});

ipcMain.on("rememberMeStateUpdated", async (event, message) => {
  store.set("login.rememberMe", message.newValue);
});

ipcMain.on("clearAuthToken", async (event, message) => {
  store.delete("login.authToken");
});

ipcMain.on("clearRefreshToken", async (event, message) => {
  store.delete("login.refreshToken");
});

ipcMain.on("about", async (event, message) => {
  if (win) {
    const details = [
      `${translate("version")}: ${pack.version}`,
      `${translate("os")}: ${getOsName()} ${os.arch()} ${os.release()}`,
    ];
    const lastUpdatedAt = store.get("app.lastUpdatedAt");
    if (lastUpdatedAt) {
      details.push(`${translate("lastUpdated")}: ${lastUpdatedAt}`);
    }
    const username = store.get("login.username");
    if (username) {
      details.push(`${translate("username")}: ${username}`);
    }
    const buttons = [translate("copyDetails"), translate("ok")];
    if (updateStatus.state == "restart") {
      details.push(`\n${translate("restartMessage")}`);
      buttons.unshift(translate("restartButton"));
    }
    else if (updateStatus.state == "error") {
      details.push(`\n${translate("updateError")}`);
      buttons.unshift(translate("websiteButton"));
    }
    const returnValue = await dialog.showMessageBox(win, {
      type: "none",
      icon: __dirname + '/gui/images/icon.png',
      title: `${pack.productName}`,
      message: `${pack.productName}`,
      detail: details.join("\n"),
      buttons,
      cancelId: buttons.length - 1,
      defaultId: buttons.length - 1,
    });
    if (updateStatus.state == "restart" && returnValue.response == 0) {
      autoUpdater.quitAndInstall();
    }
    if (updateStatus.state == "error" && returnValue.response == 0) {
      shell.openExternal(config.webClassic);
    }
    else if (returnValue.response == buttons.length - 2) {
      clipboard.writeText(details.join("\n"));
    }
  }
});

const getOsName = () => {
  switch (os.platform()) {
    case "win32": return "Windows";
    case "darwin": return "macOS";
    case "linux": return "Linux";
    default: return "Unknown";
  }
};

const getSystemData = () => {
  const language = store.get("login.language") || app.getLocale().split("-")[0];
  translation.setLanguage(language);
  return {
    version: pack.version,
    platform: os.platform(),
    platformRelease: os.release(),
    language,
    affiliateCode: store.get("login.affiliateCode") || "",
  };
};

const getDf = async () => {
  let df = store.get("login.df");
  if (df === undefined) {
    try {
      df = await machineId({original: true});
    }
    catch (err) {
      log("debugError", JSON.stringify(err));
      df = uuidv4();
    }
    store.set("login.df", df);
  }
  return df;
};

// webview ready
ipcMain.on("ready", () => {
  webview.send("postSystemData", getSystemData());
  const screenState = store.get("window.state");
  webview.send("screenChange", screenState);
});
// renderer process ready
ipcMain.on("winReady", () => {
  win.webContents.send("postSystemData", getSystemData());
});

const handleKey = event => {
  const platform = getSystemData().platform;
  // fullscreen
  if (
    (event.key === "Enter" && event.altKey && platform === "win32")
    || (event.key === "F11" && platform === "win32")
    || (event.key === "f" && event.ctrlKey && event.metaKey && platform === "darwin")
  ) {
    win.setFullScreen(!win.isFullScreen());
  }
  // quit
  else if (
    (event.key === "q" && event.ctrlKey && platform === "win32")
    || (event.key === "F4" && event.altKey && platform === "win32")
    || (event.key === "q" && event.metaKey && platform === "darwin")
  ) {
    app.quit();
  }
};

ipcMain.on("openExternal", (event, message) => {
  // open http/https url in default browser
  if (['https:', 'http:'].includes(new URL(message.url).protocol)) {
    shell.openExternal(message.url);
  }
});

ipcMain.on("keyEvent", (event, message) => handleKey(message));

// UPDATE LOCAL DATA
//  before 0.3.0 full screen was not stored in window.state
if (store.get("window.fullscreen")) {
  store.set("window.state", "fullScreen");
  store.delete("window.fullscreen");
}

// Handle system commands
ipcMain.on("systemCommand", (event, message) => {
  if (message.command === "toggleFullScreen") {
    win.setFullScreen(!win.isFullScreen());
  }
  else if (message.command === "exit") {
    app.quit();
  }
  else if (message.command === "print") {
    printWindow = new BrowserWindow({
      icon: __dirname + '/gui/images/icon.png',
      enableLargerThanScreen: true,
      x: 0,
      y: 0,
      useContentSize: true,
      resizable: false,
      webPreferences: {
        contextIsolation: true,
        nodeIntegration: false,
        preload: path.join(__dirname, "gui/printPreload.js"),
      },
      fullscreen: false,
      fullscreenable: false,
      backgroundColor: "#FFFFFF",
    });
    // Bug on windows where you cant spawn a window larger than the max monitor size
    // https://github.com/electron/electron/issues/4932
    printWindow.setSize(2480, 3508);
    if (config.showTools) {
      printWindow.webContents.openDevTools();
    }
    else {
      printWindow.hide();
    }
    printWindow.loadURL(`file://${__dirname}/gui/print.html`);

    ipcMain.on("readyForImage", event => {
      log("debug", "Got readyForImage, sending image.");
      event.sender.send("setImage", {
        image: message.image,
        width: message.width,
        height: message.height,
      });
      printWindow.webContents.print({silent: false, printBackground: false, deviceName: ""});
    });

    ipcMain.on("closePrintWindow", event => {
      printWindow.close();
    });
  }
});

const translate = (phrase) => {
  const {error, value} = translation.translate(phrase);
  if (error) {
    log("warn", error);
  }
  return value;
};

ipcMain.on("translate", (event, message) => {
  if (win) {
    win.webContents.send("translate", {
      phrase: message.phrase,
      requestId: message.requestId,
      value: translate(message.phrase),
    });
  }
});

// add flash plugin
app.commandLine.appendSwitch("ppapi-flash-path", path.join(__dirname, `${config.pluginPath}${config.pluginName}`));

app.whenReady().then(() => {
  const minWidth = 900;
  const minHeight = 550;

  // Create the browser window.
  win = new BrowserWindow({
    icon: __dirname + '/gui/images/icon.png',
    minWidth,
    minHeight,
    // 160% min size
    width: store.get("window.width") || 1440,
    height: store.get("window.height") || 880,
    x: store.get("window.x") || 0,
    y: store.get("window.y") || 0,
    useContentSize: true,
    resizable: true,
    webPreferences: {
      contextIsolation: true,
      webviewTag: true,
      nodeIntegration: false,
      preload: path.join(__dirname, "gui/preload.js"),
      plugins: true,
    },
    fullscreen: store.get("window.state") === "fullScreen",
    fullscreenable: true,
    backgroundColor: "#F5C86D",
  });
  win.setMenu(null);
  const winState = store.get("window.state");
  if (winState === "fullScreen") {
    win.setFullScreen(true);
  }
  else if (winState === "maximized") {
    win.maximize();
  }

  if (config.clearCache) {
    win.webContents.session.clearCache(() => {});
  }

  loadClient();
  if (config.showTools) {
    win.webContents.openDevTools();
  }

  win.on("closed", () => {
    win = null;
    if (printWindow) {
      printWindow.close();
    }
  });

  win.on("resize", () => {
    const bounds = win.getBounds();
    store.set("window.width", bounds.width);
    store.set("window.height", bounds.height);
  });
});

app.on("window-all-closed", () => {
  app.quit();
});
