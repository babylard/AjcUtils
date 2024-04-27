"use strict";

(() => {
  let forgotBlocked = false;

  const forgotPassword = () => {
    if (forgotBlocked) {
      return;
    }
    forgotBlocked = true;

    const modal = document.createElement("ajd-forgot-password-modal");
    modal.addEventListener("close", event => {
      document.getElementById("modal-layer").removeChild(modal);
      forgotBlocked = false;
    });
    document.getElementById("modal-layer").appendChild(modal);
  };

  customElements.define("ajd-login-screen", class extends HTMLElement {
    static get observedAttributes() {
      return [];
    }

    constructor() {
      super();

      this.attachShadow({mode: "open"}).innerHTML = `
        <style>
          :host {
            width: 100vw;
            height: calc(100vh - 2px);
            display: grid;
            grid-template: 1fr 590px 1fr / 1fr 936px 1fr;
            grid-template-areas: ". . button-tray"
                                 ". box ."
                                 ". . .";
            background-color: rgba(239, 234, 221, 0);
            transition: background-color 0.2s;
          }

          .hidden {
            display: none !important;
          }

          #box-background {
            grid-area: box;
            border-image: url(images/electron_login/log_bg_page.svg) 0 fill stretch;
            opacity: 1;
            transition: opacity 0.2s;
          }

          @media (max-width: 950px), (max-height: 590px) {
            #box-background {
              z-index: -1;
              opacity: 0;
            }

            :host {
              background-color: rgba(239, 234, 221, 1);
            }
          }

          #box {
            grid-area: box;
            display: flex;
            justify-content: center;
            border-image: url(images/electron_login/login_image.svg) 0 fill stretch;
            padding: 175px 70px 50px;
          }

          #login-container {
            display: flex;
            flex-direction: column;
            align-items: center;
          }

          #login-container > * {
            margin-bottom: 9px;
          }

          #login-image {
            user-select: none;
            pointer-events: none;
            grid-area: left;
          }

          #need-account {
            user-select: none;
            pointer-events: none;
            font-size: 12px;
            line-height: 18px;
            letter-spacing: -0.25px;
            color: #6E4B37;
            font-family: CCDigitalDelivery;
            font-weight: bold;
          }

          #player-login-text {
            color: #684A26;
            font-family: Tiki-Island;
            font-size: 36px;
            text-shadow: 1px 2px 0px rgba(2, 2, 2, 0.2);
          }

          #login-btn-container {
            display: grid;
            grid-template: 1fr / 1fr fit-content(100%) 1fr;
            grid-template-areas: "left mid right";
            align-items: center;
          }

          #log-in-btn {
            grid-area: mid;
            padding: 6px 24px;
          }

          @keyframes fade {
            0%,100% { opacity: 0 }
            50% { opacity: 1 }
          }

          @keyframes spin {
            from {
              transform: rotate(0deg);
            }
            to {
              transform: rotate(-360deg);
            }
          }

          #spinner {
            margin-left: 10px;
            grid-area: left;
            height: 90%;
            opacity: 0;
            transition: opacity .5s;
            animation: spin 1500ms linear infinite;
          }

          #spinner.show {
            opacity: 1;
          }

          ajd-text-input {
            width: 100%;
            border-radius: 25px;
            border: #BFAE92 2px solid;
          }

          #remember-me-cb {
            font-size: 15px;
            letter-spacing: -1px;
            font-weight: bold;
          }

          #forgot-password-link {
            font-size: 12px;
            line-height: 14px;
            letter-spacing: .25px;
            color: #CC6C2B;
            text-decoration: none;
            user-select: none;
            cursor: pointer;
            font-family: CCDigitalDelivery;
          }

          .vertical-spacer {
            height: 2px;
            width: 75%;
            border-bottom: #cea054 2px solid;
          }

          #forgot-password-link {
            letter-spacing: -0.5px;
          }

          #forgot-password-link:hover {
            text-decoration: underline;
          }

          #create-account-btn {
            font-size: 24px;
            padding: 4px 12px;
          }

          #version {
            position: absolute;
            right: 10px;
            bottom: 10px;
            display: grid;
            grid-template-columns: 1fr 24px;
          }

          #version:hover {
            text-decoration: underline;
          }

          #version-link {
            font-size: 16px;
            line-height: 24px;
            letter-spacing: -0.5px;
            color: #684A26;
            text-decoration: none;
            user-select: none;
            cursor: pointer;
            font-family: CCDigitalDelivery;
          }

          #version-status-icon {
            background: url(images/core/core_form_input_status_icn_sprite.svg);
            background-repeat: no-repeat;
            background-size: 80px;
            width: 20px;
            height: 20px;
            opacity: 0.0;
          }

          #version-status-icon.check {
            background-position: -20px 0px;
            animation: spin 1500ms linear infinite;
            opacity: 1.0;
            transition-property: opacity;
            transition-duration: 0.5s;
          }

          #version-status-icon.download {
            opacity: 1.0;
          }

          #version-status-icon.restart {
            background-position: -40px 0px;
            opacity: 0.0;
            animation: fade 1.5s ease-out infinite;
          }

          #version-status-icon.error {
            background-position: -60px 0px;
            opacity: 0.0;
            animation: fade 1.5s ease-out infinite;
          }

          #button-tray {
            grid-area: button-tray;
            display: flex;
            flex-direction: row;
            justify-content: flex-end;
          }
          #button-tray ajd-button {
            width: 54px;
            height: 54px;
          }
        </style>
        <div id="box-background"></div>
        <div id="button-tray" class="hidden">
          <ajd-button graphic="UI_fullScreen" id="expand-button">
          </ajd-button>
          <ajd-button graphic="UI_power" id="close-button">
          </ajd-button>
        </div>
        <div id="box">
          <div id="login-container">
            <div id="player-login-text">playerLogin</div>
            <ajd-text-input id="username-input" placeholder="username" type="text"></ajd-text-input>
            <ajd-text-input id="password-input" placeholder="password" type="password"></ajd-text-input>
            <ajd-checkbox id="remember-me-cb" text="rememberMeText"></ajd-checkbox>
            <div id="login-btn-container">
              <ajd-bubble-button id="log-in-btn" text="login"></ajd-bubble-button>
              <img id="spinner" src="images/electron_login/log_spinner.svg"></img>
            </div>
            <a id="forgot-password-link">forgotPassword</a>
            <div class="vertical-spacer"></div>
            <div id="need-account">needAccount?</div>
            <ajd-bubble-button id="create-account-btn" text="createAnimal"></ajd-bubble-button>
          </div>
        </div>
        <div id="version">
          <a id="version-link">0.0.0</a>
          <ajd-progress-ring id="version-status-icon" stroke-color="#64cc4d" stroke-width="3" radius="11"></ajd-progress-ring>
        </div>
      `;

      this._authToken = null;
      this._refreshToken = null;
      this._otp = null;
      this._isFakePassword = false;
      this._version = "";

      this.loginSpinnerElem = this.shadowRoot.getElementById("spinner");
      this.loginSpinnerElem.addEventListener("click", event => {
        if (globals.userAbortController) globals.userAbortController.abort();
      });

      this.versionElem = this.shadowRoot.getElementById("version");
      this.versionElem.addEventListener("click", () => {
        window.ipc.send("about");
      });

      this.versionLinkElem = this.shadowRoot.getElementById("version-link");
      this.versionStatusIconElem = this.shadowRoot.getElementById("version-status-icon");

      window.ipc.on("autoUpdateStatus", (event, data) => {
        for (const state of ["check", "download", "restart", "error"]) {
          this.versionStatusIconElem.classList.remove(state);
          if (state == data.state) {
            this.versionStatusIconElem.classList.add(data.state);
          }
        }
        this.setProgress(data.progress || null);
      });

      this.usernameInputElem = this.shadowRoot.getElementById("username-input");
      this.usernameInputElem.addEventListener("keydown", event => event.key === "Enter" ? this.logIn() : "");
      this.usernameInputElem.addEventListener("input", event => {
        if (this.authToken) {
          this.clearAuthToken();
        }
        if (this.refreshToken) {
          this.clearRefreshToken();
        }
      });
      this.passwordInputElem = this.shadowRoot.getElementById("password-input");
      this.passwordInputElem.addEventListener("keydown", event => event.key === "Enter" ? this.logIn() : "");
      this.passwordInputElem.addEventListener("input", event => {
        if (this.isFakePassword) {
          this.isFakePassword = false;
        }
        if (this.authToken) {
          this.clearAuthToken();
        }
        if (this.refreshToken) {
          this.clearRefreshToken();
        }
      });
      this.rememberMeElem = this.shadowRoot.getElementById("remember-me-cb");
      this.rememberMeElem.addEventListener("click", event => {
        window.ipc.send("rememberMeStateUpdated", {newValue: this.rememberMeElem.value});
      });

      this.forgotPasswordLinkElem = this.shadowRoot.getElementById("forgot-password-link");
      this.forgotPasswordLinkElem.addEventListener("click", () => {
        if (this.loginBlocked) {
          return;
        }
        forgotPassword();
      });

      this.needAccountElem = this.shadowRoot.getElementById("need-account");
      this.createAnAnimalTextElem = this.shadowRoot.getElementById("create-an-animal");
      this.playerLoginTextElem = this.shadowRoot.getElementById("player-login-text");

      this.createAccountElem = this.shadowRoot.getElementById("create-account-btn");
      this.createAccountElem.addEventListener("click", async () => {
        if (this.loginBlocked) {
          return;
        }

        this.loginBlocked = true;
        try {
          const flashVars = await globals.getFlashVarsFromWeb();

          Object.assign(
            flashVars,
            globals.getClientData(),
            {
              locale: globals.language,
              webRefPath: "create_account",
            },
            globals.affiliateCode ? {
              affiliate_code: globals.affiliateCode,
            } : {},
          );

          this.dispatchEvent(new CustomEvent("loggedIn", {detail: {flashVars}}));
        }
        catch (err) {
          globals.reportError("webClient", `Error creating account: ${err.stack || err.message}`);
          if (err.name != "Aborted") {
            window.alert("Something went wrong :(");
          }
          this.loginBlocked = false;
        }
      });

      this.logInButtonElem = this.shadowRoot.getElementById("log-in-btn");
      this.logInButtonElem.addEventListener("click", () => this.logIn());

      this.expandButtonElement = this.shadowRoot.getElementById("expand-button");
      this.expandButtonElement.addEventListener("click", event => {
        window.ipc.send("systemCommand", {command: "toggleFullScreen"});
      });

      this.closeButtonElement = this.shadowRoot.getElementById("close-button");
      this.closeButtonElement.addEventListener("click", event => {
        window.ipc.send("systemCommand", {command: "exit"});
      });

      window.ipc.on("screenChange", (event, state) => {
        const buttonTray = this.shadowRoot.getElementById("button-tray");
        if (state === "fullScreen" && globals.systemData.platform === "win32") {
          buttonTray.classList.remove("hidden");
        }
        else {
          buttonTray.classList.add("hidden");
        }
      });
    }

    async logIn(isRetry = false) {
      if (!isRetry && this.loginBlocked) {
        return;
      }
      this.loginBlocked = true;

      try {
        // TODO: set busy indicators
        this.usernameInputElem.error = "";
        this.passwordInputElem.error = "";

        let authResult;
        if (this.authToken) {
          authResult = await globals.authenticateWithAuthToken(this.authToken);
        }
        else if (this.refreshToken) {
          authResult = await globals.authenticateWithRefreshToken(this.refreshToken, this.otp);
        }
        else {
          if (!this.username.length) {
            throw new Error("EMPTY_USERNAME");
          }
          if (!this.password.length) {
            throw new Error("EMPTY_PASSWORD");
          }
          authResult = await globals.authenticateWithPassword(this.username, this.password, this.otp);
        }
        this.otp = null;
        const {userData, flashVars} = authResult;
        const data = {
          username: userData.username,
          authToken: userData.authToken,
          refreshToken: userData.refreshToken,
          accountType: userData.accountType,
          language: userData.language,
          rememberMe: this.rememberMeElem.value,
        };
        if (userData.authToken) {
          this.authToken = userData.authToken;
        }
        if (userData.refreshToken) {
          this.refreshToken = userData.refreshToken;
        }
        window.ipc.send("loginSucceeded", data);

        this.dispatchEvent(new CustomEvent("loggedIn", {detail: {flashVars}}));
      }
      catch (err) {
        if (err.message) {
          switch (err.message) {
            case "SUSPENDED":
              this.usernameInputElem.error = await globals.translate("userSuspended");
              break;
            case "BANNED":
              this.usernameInputElem.error = await globals.translate("userBanned");
              break;
            case "LOGIN_ERROR":
              this.usernameInputElem.error = await globals.translate("loginError");
              break;
            case "WRONG_CREDENTIALS":
              this.passwordInputElem.error = await globals.translate("wrongCredentials");
              break;
            case "EMPTY_USERNAME":
              this.usernameInputElem.error = await globals.translate("usernameRequired");
              break;
            case "EMPTY_PASSWORD":
              this.passwordInputElem.error = await globals.translate("emptyPassword");
              break;
            case "USER_RENAME_NEEDED":
            case "OTP_NEEDED":
              // nothing to see here
              break;
            case "AUTH_TOKEN_EXPIRED":
              this.clearAuthToken();
              if (this.canRetry()) {
                setTimeout(() => this.logIn(true), 1000);
              }
              else {
                this.isFakePassword = false;
                this.password = "";
              }
              break;
            case "REFRESH_TOKEN_EXPIRED":
              this.clearRefreshToken();
              if (this.canRetry()) {
                setTimeout(() => this.logIn(true), 1000);
              }
              else {
                this.isFakePassword = false;
                this.password = "";
              }
              break;
            default: {
              globals.reportError("webClient", `Error logging in: ${err.stack || err.message}`);
              if (err.name != "Aborted") {
                window.alert("Something went wrong :(");
              }
            } break;
          }
        }
        else {
          globals.reportError("webClient", `Error logging in: ${err}`);
          if (err.name != "Aborted") {
            window.alert("Something went wrong :(");
          }
        }
        // TODO: reset busy indicators
        this.loginBlocked = false;
      }
    }

    canRetry() {
      return (this.authToken !== null || this.refreshToken !== null ||
        (this.username && this.password && !this.isFakePassword));
    }

    get loginBlocked() {
      return this.logInButtonElem.disabled || this.createAccountElem.disabled;
    }

    set loginBlocked(val) {
      if (val) {
        globals.userAbortController = new AbortController();
        this.logInButtonElem.disabled = true;
        this.createAccountElem.disabled = true;
        this.loginSpinnerElem.classList.add("show");
      }
      else {
        setTimeout(() => {
          globals.userAbortController = null;
          this.logInButtonElem.disabled = false;
          this.createAccountElem.disabled = false;
          this.loginSpinnerElem.classList.remove("show");
        }, 1000);
      }
    }

    get username() {
      return this.usernameInputElem.value;
    }

    set username(val) {
      this.usernameInputElem.value = val;
    }

    get password() {
      return this.passwordInputElem.value;
    }

    set password(val) {
      this.passwordInputElem.value = val;
    }

    get isFakePassword() {
      return this._isFakePassword;
    }

    set isFakePassword(val) {
      this._isFakePassword = val;
      if (val) {
        this.password = "FAKE_PASSWORD";
      }
    }

    get rememberMe() {
      return this.rememberMeElem.value;
    }

    set rememberMe(val) {
      this.rememberMeElem.value = val;
    }

    get version() {
      return this._version;
    }

    set version(val) {
      this._version = val;
      this.versionLinkElem.innerHTML = `v${val}`;
    }

    setProgress(progress) {
      if (progress === null) {
        this.versionStatusIconElem.setAttribute("progress", 0);
        this.version = this._version;
      }
      else {
        this.versionStatusIconElem.setAttribute("progress", progress);
        this.versionLinkElem.innerHTML = `${progress}%`;
      }
    }

    get otp() {
      return this._otp;
    }

    set otp(val) {
      this._otp = val;
    }

    get authToken() {
      return this._authToken;
    }

    set authToken(val) {
      this._authToken = val;
    }

    clearAuthToken() {
      this.authToken = null;
      window.ipc.send("clearAuthToken");
    }

    get refreshToken() {
      return this._refreshToken;
    }

    set refreshToken(val) {
      this._refreshToken = val;
    }

    clearRefreshToken() {
      this.refreshToken = null;
      window.ipc.send("clearRefreshToken");
    }

    async localize() {
      this.usernameInputElem.placeholder = await globals.translate("username");
      this.passwordInputElem.placeholder = await globals.translate("password");
      this.rememberMeElem.text = await globals.translate("rememberMeText");
      this.logInButtonElem.text = await globals.translate("login");
      this.forgotPasswordLinkElem.innerText = await globals.translate("forgotPassword");
      this.needAccountElem.innerText = await globals.translate("needAccount");
      this.createAccountElem.text = await globals.translate("createAccount");
      this.playerLoginTextElem.innerText = await globals.translate("playerLogin");
      this.playerLoginTextElem.style.fontSize = `${Math.min(646 / this.playerLoginTextElem.innerText.length, 36)}px`;
    }
  });
})();
