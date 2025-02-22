﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.API.Networking.Status;
using CSM.Commands.Data.Internal;
using CSM.Container;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;

namespace CSM.Panels
{
    /// <summary>
    ///     Displays a chat on the users screen. Allows a user to send messages
    ///     to other players and view important events such as server startup and
    ///     connections.
    /// </summary>
    public class ChatLogPanel: UIPanel, IChat
    {
        private UILabel _messageBox;
        private UILabel _title;
        private UITextField _chatText;
        private UITextField _chatTextChirper;
        private UIResizeHandle _resize;
        private UIScrollablePanel _scrollablepanel;
        private UIDragHandle _draghandle;

        private readonly List<ChatCommand> _chatCommands;

        private float _timeoutCounter;

        private bool UseChirper => CSM.Settings.UseChirper;

        private UITextField ChatTextChirper {
            get
            {
                if (_chatTextChirper)
                {
                    return _chatTextChirper;
                }
                _chatTextChirper = (UITextField) ChirpPanel.instance.component.AddUIComponent(typeof(UITextField));
                _chatTextChirper.width = 350;
                _chatTextChirper.height = 30;
                _chatTextChirper.position = new Vector2(15, -155);
                _chatTextChirper.atlas = UiHelpers.GetAtlas("Ingame");
                _chatTextChirper.normalBgSprite = "TextFieldPanelHovered";
                _chatTextChirper.builtinKeyNavigation = true;
                _chatTextChirper.isInteractive = true;
                _chatTextChirper.readOnly = false;
                _chatTextChirper.horizontalAlignment = UIHorizontalAlignment.Left;
                _chatTextChirper.eventKeyDown += OnChatKeyDown;
                _chatTextChirper.textColor = new Color32(0, 0, 0, 255);
                _chatTextChirper.padding = new RectOffset(6, 6, 6, 6);
                _chatTextChirper.selectionSprite = "EmptySprite";
                _chatTextChirper.name = "ChatLogChirperChatText";
                _chatTextChirper.isVisible = false;
                return _chatTextChirper;
            }
        }

        public ChatLogPanel()
        {
            Chat.Instance = this;

            if (UseChirper)
            {
                isVisible = false;
            }

            _chatCommands = new List<ChatCommand>
            {
                new ChatCommand("help", "Displays information about which commands can be displayed.", (command) =>
                {
                    foreach (ChatCommand c in _chatCommands)
                    {
                        PrintGameMessage($"/{c.Command} : {c.Description}");
                    }
                }),
                new ChatCommand("version", "Displays version information about the mod and game.", (command) =>
                {
                    PrintGameMessage("Mod Version  : " + Assembly.GetAssembly(typeof(CSM)).GetName().Version);
                    PrintGameMessage("Game Version : " + BuildConfig.applicationVersion);
                }),
                new ChatCommand("support", "Display support links for the mod.", (command) =>
                {
                    PrintGameMessage("Website : https://citiesskylinesmultiplayer.com");
                    PrintGameMessage("GitHub : https://github.com/CitiesSkylinesMultiplayer/CSM");
                    PrintGameMessage("Discord : https://discord.gg/RjACPhd");
                    PrintGameMessage("Steam Workshop : https://steamcommunity.com/sharedfiles/filedetails/?id=1558438291");
                }),
                new ChatCommand("players", "Displays a list of players connected to the server", (command) =>
                {
                    if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.None)
                    {
                        PrintGameMessage("You are not hosting or connected to any servers.");
                        return;
                    }

                    foreach (string player in MultiplayerManager.Instance.PlayerList)
                    {
                        PrintGameMessage(player);
                    }
                }),
                new ChatCommand("donate", "Find out how to support the mod developers", (command) =>
                {
                    PrintGameMessage("Want to help support the mod?");
                    PrintGameMessage("Help develop the mod here: https://github.com/CitiesSkylinesMultiplayer/CSM");
                    PrintGameMessage("Donate to the developers here: https://www.patreon.com/CSM_MultiplayerMod");
                }),
                new ChatCommand("clear", "Clear everything from the chat log.", (command) =>
                {
                    if (UseChirper)
                    {
                        ChirpPanel.instance.ClearMessages();
                    }
                    else
                    {
                        _messageBox.text = "";
                    }
                }),
                new ChatCommand("open-log", "Opens the multiplayer log.", (command) =>
                {
                    Process.Start(Log.Instance.CurrentLogFile);
                }),
                new ChatCommand("sync", "Redownloads the entire save", (command) =>
                {
                    if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                    {
                        if (MultiplayerManager.Instance.GameBlocked)
                        {
                            PrintGameMessage("Please wait until the currently joining player is connected!");
                        }
                        else
                        {
                            PrintGameMessage("Requesting the save game from the server");

                            MultiplayerManager.Instance.CurrentClient.Status =
                                ClientStatus.Downloading;
                            MultiplayerManager.Instance.BlockGameReSync();

                            Command.SendToServer(new RequestWorldTransferCommand());
                        }
                    }
                    else
                    {
                        PrintGameMessage("You are the server");
                    }
                })
            };
        }

        public override void Update()
        {
            // Prevent opening the chat while typing in text fields or the pause menu is opened
            bool allowOpen = !(UIView.HasModalInput() || UIView.HasInputFocus());

            // Check if chat is allowed to open.
            if (allowOpen && (!isVisible || !_chatText.hasFocus))
            {
                // On T is pressed.
                if (Input.GetKeyDown(KeyCode.T))
                {
                    // Open the chat window.
                    OpenChatWindow();
                }
                // On forward slash is pressed.
                else if (Input.inputString == "/")
                {
                    if (UseChirper)
                    {
                        // Prefix chat input.
                        ChatTextChirper.text = "/";
                        // Move the cursor to the end of field.
                        ChatTextChirper.MoveToEnd();
                    }
                    else
                    {
                        _chatText.text = "/";
                        _chatText.MoveToEnd();
                    }

                    // Open the chat window.
                    OpenChatWindow();
                }
            }

            // Increment the timeout counter if panel is visible
            if (isVisible && !_chatText.hasFocus) _timeoutCounter += Time.deltaTime;

            // If timeout counter has timed out, hide chat.
            if (_timeoutCounter > 5) {
                isVisible = false;
                _timeoutCounter = 0;
            }

            base.Update();
        }

        private void OpenChatWindow()
        {
            if (UseChirper)
            {
                ChatTextChirper.Show();
                ChatTextChirper.Focus();
                if (ChirpPanel.instance.isShowing)
                {
                    // Don't close Chirper automatically if already open
                    ReflectionHelper.SetAttr(ChirpPanel.instance, "m_Timeout", 0f);
                }
                else
                {
                    ChirpPanel.instance.Show(0);
                    Vector3 posInit = ChatTextChirper.position;
                    posInit.y = -45f;
                    ChatTextChirper.position = posInit;
                    ValueAnimator.Animate("ChirpPanelChatX", val =>
                    {
                        ChatTextChirper.width = val;
                    }, new AnimatedFloat(25, 350, ChirpPanel.instance.m_ShowHideTime, ChirpPanel.instance.m_ShowEasingType), () =>
                    {
                        if (!ChirpPanel.instance.isShowing)
                            return;

                        ValueAnimator.Animate("ChirpPanelChatY", val =>
                        {
                            Vector3 pos = ChatTextChirper.position;
                            pos.y = val;
                            ChatTextChirper.position = pos;
                        }, new AnimatedFloat(-45f, -155, ChirpPanel.instance.m_ShowHideTime, ChirpPanel.instance.m_ShowEasingType));
                    });
                }
            }
            else
            {
                isVisible = true;
                _chatText.Focus();

                // Reset the timeout counter
                _timeoutCounter = 0;

                // Scroll to bottom of the panel.
                _scrollablepanel.ScrollToBottom();
            }
        }

        public override void Start()
        {
            // Generates the following UI:
            // /NAME-----------\ <-- UIDragHandle
            // |---------------|-|
            // |               | |<-- _messageBox, _getscrollablepanel
            // |               | |
            // |---------------| |
            // |               | |<-- _chatText
            // |---------------|-|
            //                 |-|<-- _resize
            //                  ^
            //                  ¦-- _scrollbar, _trackingsprite, _trackingthumb

            backgroundSprite = "GenericPanel";
            name = "ChatLogPanel";
            color = new Color32(22, 22, 22, 240);

            // Activates the dragging of the window
            _draghandle = AddUIComponent<UIDragHandle>();
            _draghandle.name = "ChatLogPanelDragHandle";

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(10.0f, view.GetScreenResolution().y - 440.0f);

            width = 500;
            height = 310;
            minimumSize = new Vector2(300, 310);

            // Add resize component
            _resize = AddUIComponent<UIResizeHandle>();
            _resize.position = new Vector2((width - 20), (-height + 10));
            _resize.width = 20f;
            _resize.height = 20f;
            _resize.color = new Color32(255, 255, 255, 255);
            _resize.backgroundSprite = "GenericTabPressed";
            _resize.name = "ChatLogPanelResize";

            // Add scrollable panel component
            _scrollablepanel = AddUIComponent<UIScrollablePanel>();
            _scrollablepanel.width = 490;
            _scrollablepanel.height = 240;
            _scrollablepanel.position = new Vector2(10, -30);
            _scrollablepanel.clipChildren = true;
            _scrollablepanel.name = "ChatLogPanelScrollablePanel";

            // Add title
            _title = AddUIComponent<UILabel>();
            _title.position = new Vector2(10, -5);
            _title.text = "Multiplayer Chat";
            _title.textScale = 0.8f;
            _title.autoSize = true;
            _title.name = "ChatLogPanelTitle";

            // Add messagebox component
            _messageBox = _scrollablepanel.AddUIComponent<UILabel>();
            _messageBox.isVisible = true;
            _messageBox.isEnabled = true;
            _messageBox.autoSize = false;
            _messageBox.autoHeight = true;
            _messageBox.width = 470;
            _messageBox.height = 240;
            _messageBox.position = new Vector2(10, -30);
            _messageBox.textScale = 0.8f;
            _messageBox.wordWrap = true;
            _messageBox.name = "ChatLogPanelMessageBox";

            // Add scrollbar component
            this.AddScrollbar(_scrollablepanel);

            // Add text field component (used for inputting)
            _chatText = (UITextField)AddUIComponent(typeof(UITextField));
            _chatText.width = width;
            _chatText.height = 30;
            _chatText.position = new Vector2(0, -280);
            _chatText.atlas = UiHelpers.GetAtlas("Ingame");
            _chatText.normalBgSprite = "TextFieldPanelHovered";
            _chatText.builtinKeyNavigation = true;
            _chatText.isInteractive = true;
            _chatText.readOnly = false;
            _chatText.horizontalAlignment = UIHorizontalAlignment.Left;
            _chatText.eventKeyDown += OnChatKeyDown;
            _chatText.textColor = new Color32(0, 0, 0, 255);
            _chatText.padding = new RectOffset(6, 6, 6, 6);
            _chatText.selectionSprite = "EmptySprite";
            _chatText.name = "ChatLogPanelChatText";

            WelcomeChatMessage();

            // Add resizable adjustments
            eventSizeChanged += (component, param) =>
            {
                _scrollablepanel.width = (width - 30);
                _scrollablepanel.height = (height - 70);
                _messageBox.width = (width - 30);
                _chatText.width = width;
                _chatText.position = new Vector3(0, (-height + 30));
                _resize.position = new Vector2((width - 20), (-height + 10));

                UIScrollbar uiScrollbar = Find<UIScrollbar>("PanelScrollBar");
                UISlicedSprite trackingSprite = Find<UISlicedSprite>("PanelTrack");

                uiScrollbar.height = _scrollablepanel.height;
                trackingSprite.size = trackingSprite.parent.size;
                uiScrollbar.position = new Vector2(width - 20, -30);
            };

            base.Start();
        }

        private void OnChatKeyDown(UIComponent component, UIKeyEventParameter eventParam)
        {
            // Reset chat timeout
            _timeoutCounter = 0;

            // Run this code when the user presses the enter key
            if (eventParam.keycode == KeyCode.Return || eventParam.keycode == KeyCode.KeypadEnter)
            {
                eventParam.Use();

                string text = UseChirper ? ChatTextChirper.text : _chatText.text;

                if (string.IsNullOrEmpty(text))
                {
                    isVisible = false;
                    base.Update();
                    return;
                }

                if (UseChirper)
                {
                    ChatTextChirper.text = string.Empty;
                    ChatTextChirper.Hide();
                    ReflectionHelper.SetAttr(ChirpPanel.instance, "m_Timeout", 6f);
                }
                else
                {
                    _chatText.text = string.Empty;
                }

                SubmitText(text);
            }
            else if (eventParam.keycode == KeyCode.Escape)
            {
                eventParam.Use();

                if (UseChirper)
                {
                    ChatTextChirper.text = string.Empty;
                    ChirpPanel.instance.Hide();
                    ChatTextChirper.Hide();
                }
                else
                {
                    _chatText.text = string.Empty;
                    isVisible = false;
                }
            }

            base.Update();
        }

        private void SubmitText(string text)
        {
            // If a command, parse it
            if (text.StartsWith("/"))
            {
                ChatCommand command = _chatCommands.Find(x => x.Command == text.TrimStart('/'));
                if (command == null)
                {
                    PrintGameMessage(Chat.MessageType.Warning, $"'{text.TrimStart('/')}' is not a valid command.");
                    return;
                }

                // Run the command
                command.Action.Invoke(text.TrimStart('/'));

                return;
            }

            // If not connected to a server / hosting a server, tell the user and return
            //if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.None)
            //{
            //    PrintGameMessage(MessageType.Warning, "You can only use the chat feature when hosting or connected.");
            //    return;
            //}

            // Get the player name
            string playerName = "Local";

            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                playerName = MultiplayerManager.Instance.CurrentClient.Config.Username;
            }
            else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                playerName = MultiplayerManager.Instance.CurrentServer.Config.Username;
            }

            // Build and send this message
            ChatMessageCommand message = new ChatMessageCommand
            {
                Username = playerName,
                Message = text
            };

            Command.SendToAll(message);

            // Add the message to the chat UI
            PrintChatMessage(playerName, text);
        }

        public void WelcomeChatMessage()
        {
            PrintGameMessage("Welcome to Cities: Skylines Multiplayer Walmart Edition!");
            PrintGameMessage("The chat can be opened by pressing T and closed by pressing escape.");
            PrintGameMessage("Join our discord server at: https://discord.gg/RjACPhd");
            PrintGameMessage("Type '/help' to see a list of commands and usage.");
            PrintGameMessage("Type '/support' to find out where to report bugs and get help.");
        }

        /// <summary>
        ///     Prints a game message to the ChatLogPanel and Chirper with MessageType.NORMAL.
        /// </summary>
        /// <param name="msg">The message.</param>
        public void PrintGameMessage(string msg)
        {
            PrintGameMessage(Chat.MessageType.Normal, msg);
        }

        /// <summary>
        ///     Prints a game message to the ChatLogPanel and Chirper.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="msg">The message.</param>
        public void PrintGameMessage(Chat.MessageType type, string msg)
        {
            PrintMessage("CSM", msg);
        }

        /// <summary>
        ///     Prints a chat message to the ChatLogPanel and Chirper.
        /// </summary>
        /// <param name="username">The name of the sending user.</param>
        /// <param name="msg">The message.</param>
        public void PrintChatMessage(string username, string msg)
        {
            PrintMessage(username, msg);
        }

        private void PrintMessage(string sender, string msg)
        {
            try
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    if (UseChirper)
                    {
                        // Write message to Chirper panel
                        ChirperMessage.ChirpPanel.AddMessage(new ChirperMessage(sender, msg), true);
                    }
                    else
                    {

                        msg = $"<{sender}> {msg}";
                        ChatLogPanel chatPanel = UIView.GetAView().FindUIComponent<ChatLogPanel>("ChatLogPanel");
                        if (chatPanel != null)
                        {
                            // Reset the timeout counter when a new message is received
                            chatPanel._timeoutCounter = 0;

                            // If the panel is closed, make sure it gets shown
                            if (!chatPanel.isVisible)
                            {
                                chatPanel.isVisible = true;
                                chatPanel.Update();
                            }
                        }

                        UILabel messageBox = UIView.GetAView().FindUIComponent<UILabel>("ChatLogPanelMessageBox");
                        if (messageBox != null)
                        {
                            // Check if the thumb is at the bottom of the scrollbar for autoscrolling
                            UIScrollbar scrollBar =
                                UIView.GetAView().FindUIComponent<UIScrollbar>("PanelScrollBar");
                            UISlicedSprite thumb =
                                UIView.GetAView().FindUIComponent<UISlicedSprite>("PanelThumb");
                            float size = (thumb.relativePosition.y + thumb.size.y);
                            bool autoScroll = Math.Abs(size - scrollBar.height) < 0.2f;

                            messageBox.text += ($"{msg}\n");

                            if (autoScroll)
                            {
                                scrollBar.minValue = scrollBar.maxValue;
                            }
                        }
                    }
                });
            }
            catch (Exception)
            {
                // IGNORE
            }
        }

        public void HideChirpText()
        {
            ChatTextChirper.Hide();
        }
    }
}
