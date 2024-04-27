"use strict";

const { ipcRenderer, contextBridge } = require("electron");

// renderer -> main
const sendWhitelist = new Set()
  .add("about")
  .add("clearAuthToken")
  .add("clearRefreshToken")
  .add("keyEvent")
  .add("loaded")
  .add("loginSucceeded")
  .add("openExternal")
  .add("ready")
  .add("rememberMeStateUpdated")
  .add("systemCommand")
  .add("translate");

// main -> renderer
const receiveWhitelist = new Set()
  .add("autoUpdateStatus")
  .add("log")
  .add("loginInfoLoaded")
  .add("postSystemData")
  .add("obtainedToken")
  .add("screenChange")
  .add("toggleDevTools")
  .add("translate");

  // allow renderer process to safely communicate with main process
contextBridge.exposeInMainWorld(
  "ipc", {
    send: (channel, ...args) => {
      if (sendWhitelist.has(channel)) {
        ipcRenderer.send(channel, ...args);
      }
    },
    on: (channel, listener) => {
      if (receiveWhitelist.has(channel)) {
        ipcRenderer.on(channel, listener);
      }
    }
  }
);

ipcRenderer.on("redirect-url", (event, url) => {
  console.log("REDIRECT");
});
