Vue.directive('long', {
    inserted(el, binding) {
        var methods = {
            timer: null,
            msg: binding.value,
            gtouchstart() {
                this.timer = setTimeout(() => {
                    this.show(this.msg);
                }, 500);
            },
            gtouchend() {
                clearTimeout(this.timer);
            },
            show() {
                alert(this.msg);
            }
        };
        el.addEventListener('mousedown', methods.gtouchstart.bind(methods));
        el.addEventListener('mouseup', methods.gtouchend.bind(methods));
    }
});

new Vue({
    el: '#app',
    data: {
        connection: null,
        myinfo: {},
        room: {
            name: "",
            description: ""
        },
        player: {},
        players: [],
        npc: {},
        npcs: [],
        messages: [],
        chats: [],
        showchat: false,
        msg: "",
        myBox: "",
        myBoxMenus: [],
        menus: [{ id: "me", name: "属性", group: "player" }, /*{ id: "status", name: "状态", group: "player" },*/ { id: "skill", name: "武功", group: "player" }, /*{ id: "achv", name: "成就", group: "player" },*/ { id: "mypack", name: "背包", group: "player" }, { id: "weapon", name: "装备", group: "player" },

            { id: "email", name: "邮箱", group: "email" },

            { id: "activityQuest1", name: "新手任务", group: "quest" }, { id: "activityQuest2", name: "主线任务", group: "quest" }, { id: "activityQuest3", name: "日常任务", group: "quest" }, { id: "activityQuest4", name: "支线任务", group: "quest" }, { id: "historyQuest", name: "已完成", group: "quest" },  
            { id: "friend", name: "好友", group: "social" }, { id: "enemy", name: "仇人", group: "social" }, { id: "master", name: "师傅", group: "social" }, { id: "couple", name: "伴侣", group: "social" }],
        myDetail: "",
        modal: {
            isShowConfirm: 0,
            type: "confirm",
            title: "",
            content: "",
            cancelText: "取消",
            confirmText: "确认",
            callback: null
        },
        timer: null,
        mainQuest: {},
        myPack: {},
        unreadEmailCount: 0,
        myEmails: null,
        myEmail: null,
        skills: [],
        mySkill:null,
        weapons: [],
        myWare: null,
        quest: null,
        myFriends: {},
        friendSkills: [],
        fightTargets:[]
    },
    computed: {
        getMenus() {
            return function (id) {
                var that = this;
                console.log("id=" + id);
                if (!that.myBox) {
                    return [];
                }
                var group = "";
                var menu, i;
                for (i in that.menus) {
                    menu = that.menus[i];
                    if (menu.id === id) {
                        group = menu.group;
                        break;
                    }
                }
                i = 0;
                var currMenus = [];
                for (i in that.menus) {
                    menu = that.menus[i];
                    if (menu.group === group) {
                        currMenus.push(menu);
                    }
                }
                return currMenus;
            };
        }, hpLength() {
            return this.myinfo.hp > this.myinfo.maxHp ? 100 : (this.myinfo.hp + 1) * 100 / (this.myinfo.maxHp + 1);     
        }, mpLength() {
            return this.myinfo.mp > this.myinfo.maxMp ? 100 : (this.myinfo.mp + 1) * 100 / (this.myinfo.maxMp + 1);
        }
    },
    methods: {
        log: function (str) {
            console && console.log(str);
        },
        logout: function () {
            var that = this;
            that.confirm("要退出吗？", function () {
                axios.post("/api/user/logout", {})
                    .then(function (response) {
                        var result = response.data;
                        if (result.status) {
                            location.href = "/";
                        } else {
                            var error = result.errorMessage || "退出失败";
                            if (result.data) {
                                error += "<br>" + result.data;
                            }
                            that.tips = error;
                        }
                    })
                    .catch(function (error) {
                        console.log(error);
                    });
            });
        },
        confirm: function (content, callback, no) {
            var that = this;
            that.modal.title = "";
            that.modal.type = "confirm";
            that.modal.isShowConfirm = 1;
            that.modal.content = content;
            that.modal.callback = callback;
            that.modal.no = no;


        },
        confirmEvent: function (type) {
            var that = this;
            if (type === "ok") {
                that.modal.callback && that.modal.callback();
            } else if (type === "no") {
                that.modal.no && that.modal.no();
            }
            that.modal.isShowConfirm = false;
        },
        dialog: function (title, content, callback) {
            var that = this;
            that.modal.type = "dialog";
            that.modal.isShowConfirm = 1;
            that.modal.title = title;
            that.modal.content = content;
            that.modal.callback = callback;
        },
        ping: function () {
            this.timer = setInterval(() => {
                connection.invoke("Ping");
            }, 1000 * 60);
        },
        joinGame: function () {
            var that = this;

            connection = that.connection;
            connection = new signalR.HubConnectionBuilder().withUrl("/hub")
                .withAutomaticReconnect([0, 3000, 5000, 10000, 15000, 30000, 300000])
                .build();

            connection.start().then(function () {
                console.log("start");
            }).catch(function (err) {
                return console.error(err.toString());
            });

            connection.on("showMessage", message => {
                console.log("showMessage:" + JSON.stringify(message));
                message.sender ? that.chats.push(message) : that.messages.push(message);
            });

            connection.on("move", result => {
                console.log("move:" + JSON.stringify(result));
                that.room = result;
            });

            connection.on("updatePlayerList", playerList => {
                console.log("updatePlayerList:" + JSON.stringify(playerList));
                that.players = playerList;
            });

            connection.on("updateNpcList", npcList => {
                console.log("updateNpcList:" + JSON.stringify(npcList));
                that.npcs = npcList;
            });

            connection.on("UpdatePlayerInfo", result => {
                console.log("UpdatePlayerInfo:" + JSON.stringify(result));
                that.myinfo = result;
            });

            connection.on("ShowNpc", result => {
                console.log("ShowNpc:" + JSON.stringify(result));
                that.myBox = "npc";
                that.npc = result;
            });

            connection.on("ShowPlayer", result => {
                console.log("ShowPlayer:" + JSON.stringify(result));
                that.myBox = "player";
                that.player = result;
            });

            connection.on("ShowMe", result => {
                console.log("ShowMe:" + JSON.stringify(result));
                that.myBox = "me";
                that.myinfo = result;
            });

            connection.on("ShowMyStatus", result => {
                console.log("ShowMyStatus:" + JSON.stringify(result));
                that.myBox = "status";
                that.myinfo = result;
            });

            connection.on("ShowMySkill", result => {
                console.log("ShowMySkill:" + JSON.stringify(result));
                that.myBox = "skill";
                that.skills = result;
            });

            connection.on("ShowSkill", result => {
                console.log("ShowSkill:" + JSON.stringify(result));
                for (var i = 0; i < that.skills.length; i++) {
                    if (that.skills[i].playerSkillId == result.playerSkillId) {
                        that.skills[i] = result;
                    }
                }
                that.mySkill = result;
                that.myDetail = "skill";
            });

            connection.on("ShowFriendSkill", result => {
                console.log("ShowFriendSkill:" + JSON.stringify(result));
                that.myBox = "friendSkill";
                that.friendSkills = result;
            });
            
            

            connection.on("UpdatePlayerStatus", result => {
                console.log("UpdatePlayerStatus:" + result);
                that.myinfo.status = result;
            });

            connection.on("offline", () => {
                connection.stop();
                connection = null;
                location.replace("/game/offline");
            });

            connection.on("ShowMainQuest", result => {
                console.log("ShowMainQuest:" + JSON.stringify(result));
                that.mainQuest = result.mainQuest;
                if (result.isFirst) {
                    that.showMainQuest();
                } 
            });

            connection.on("ShowQuest", result => {
                console.log("ShowQuest:" + JSON.stringify(result));
                that.quest = result;
                that.myDetail = "quest";
            });

            connection.on("ShowMyPack", result => {
                console.log("ShowMyPack:" + JSON.stringify(result));
                that.myBox = "mypack";
                that.myPack = result;
            });

            connection.on("ShowMyWeapon", result => {
                console.log("ShowMyWeapon:" + JSON.stringify(result));
                that.myBox = "weapon";
                that.weapons = result;
            });

            connection.on("LoadWare", result => {
                console.log("LoadWare:" + JSON.stringify(result));
                for (var i = 0; i < that.myPack.wares.length; i++) {
                    if (that.myPack.wares[i].playerWareId == result.playerWareId) {
                        that.myPack.wares[i] = result;
                        that.myWare = result;
                    }
                }
            });


            connection.on("UnLoadWare", result => {
                console.log("UnLoadWare:" + JSON.stringify(result));
                for (var i = 0; i < that.myPack.wares.length; i++) {
                    if (that.myPack.wares[i].playerWareId == result.playerWareId) {
                        that.myPack.wares[i] = result;
                        that.myWare = result;
                    }
                }
            });

            connection.on("ShowWare", result => {
                console.log("ShowWare:" + JSON.stringify(result));
                for (var i = 0; i < that.myPack.wares.length; i++) {
                    if (that.myPack.wares[i].playerWareId == result.playerWareId) {
                        that.myPack.wares[i] = result;

                        that.myWare = result;
                        that.myDetail = "ware";
                    }
                }
               
            });

            connection.on("DropWare", result => {
                console.log("DropWare:" + JSON.stringify(result));

                that.myPack.wares = that.myPack.wares.filter(item => {
                    if (result.playerWareId !== item.playerWareId) {
                        return true;
                    }
                });
                that.myWare = null;
                that.myDetail = "";
            });
            
            

            connection.on("UpdateUnreadEmailCount", result => {
                console.log("UpdateUnreadEmailCount:" + result);
                that.unreadEmailCount = result;
            });

            connection.on("ShowEmail", result => {
                console.log("ShowEmail:" + JSON.stringify(result));
                that.myBox = "email";
                that.myEmails = result;
            });

            connection.on("RemoveEmail", result => {
                console.log("RemoveEmail:" + result);

                that.showEmail(1);
                that.myEmail = null;
                that.myDetail = "";
            });

            connection.on("ShowEmailDetail", result => {
                console.log("ShowEmailDetail:" + JSON.stringify(result));

                for (var i = 0; i < that.myEmails.data.length; i++) {
                    if (that.myEmails.data[i].id == result.id) {
                        that.myEmails.data[i] = result;
                    }
                }

                that.myEmail = result;
                that.myDetail = "email";
            });

            connection.on("ShowQuests", result => {
                console.log("ShowQuests:" + JSON.stringify(result));
                that.myBox = "activityQuest" + result.type;
                that.myQuests = result.quests;
            });

            connection.on("ShowHistoryQuests", result => {
                console.log("ShowHistoryQuests:" + JSON.stringify(result));
                that.myBox = "historyQuest";
                that.myHistoryQuests = result;
            });

            connection.on("ShowFriend", result => {
                console.log("ShowFriend:" + JSON.stringify(result));
                that.myBox = "friend";
                that.myFriends = result;
            });

            connection.on("ShowBox", result => {
                console.log("ShowBox:" + JSON.stringify(result));
                that.myBox = result.boxName;
            });

            connection.on("AddFightingTarget", result => {
                console.log("AddFightingTarget:" + JSON.stringify(result));
                var hasTarget = false;
                for (var i = 0; i < that.fightTargets.length; i++) {
                    if (that.fightTargets[i].targetId == result.targetId && that.fightTargets[i].targetType == result.targetType) {
                        that.fightTargets[i].hp = result.hp;
                        that.fightTargets[i].mp = result.mp;
                        hasTarget = true;
                    }
                }
                if(!hasTarget){
                    that.fightTargets.push(result);
                }
            });
            
            
        },
        move: function (roomId) {
            if (roomId > 0) {
                connection.invoke("Move", { roomId: roomId });
            }
        },
        send: function () {
            var that = this;
            if (!that.msg) {
                return;
            }
            connection.invoke("Send", { content: that.msg, channel: "", receiver: "" });
            that.msg = "";
            this.$refs.msg.focus();
        },
        showPlayer: function (id) {
            id > 0 && connection.invoke("ShowPlayer", { playerId: id });
        },
        showNpc: function (id) {
            id > 0 && connection.invoke("ShowNpc", { npcId: id });
        },
        showMe: function () {
            connection.invoke("ShowMe");
        },
        showStatus: function () {
            connection.invoke("ShowMyStatus");
        },
        showMyPack: function () {
            connection.invoke("ShowMyPack");
        },
        showSkill: function () {
            connection.invoke("ShowMySkill");
        },
        showWeapon: function () {
            connection.invoke("ShowMyWeapon");
        },
        showEmail: function (pageIndex) {
            connection.invoke("ShowEmail", { pageIndex: pageIndex||1 });
        },
        showEmailDetail: function (id) {
            connection.invoke("ShowEmailDetail", { playerEmailId: id});
        },
        showMyQuest: function (type) {
            connection.invoke("ShowMyQuest", { type: type||1 });
        },
        showMyHistoryQuest: function () {
            connection.invoke("ShowMyHistoryQuest");
        },
        showQuestDetail: function (id) {
            connection.invoke("ShowQuestDetail", { questId: id });
        },
        showSkillDetail: function (id) {
            connection.invoke("ShowSkillDetail", { playerSkillId: id });
        },
        learnSkill: function (id) {
            connection.invoke("LearnSkill", { playerSkillId: id });
        },

        search: function () {
            connection.invoke("Search");
        },
        stopAction: function () {
            connection.invoke("StopAction");
        },
        meditate: function () {
            connection.invoke("Meditate");
        },
        exert: function () {
            connection.invoke("Exert");
        },
        fish: function () {
            connection.invoke("Fish");
        },
        dig: function () {
            connection.invoke("Dig");
        },
        collect: function () {
            connection.invoke("Collect");
        },
        cut: function () {
            connection.invoke("Cut");
        },
        hunt: function () {
            connection.invoke("Hunt");
        },
        clickMenu: function (id) {
            switch (id) {
                case "me":
                    this.showMe();
                    break;
                case "status":
                    this.showStatus();
                    break;
                case "mypack":
                    this.showMyPack();
                    break;
                case "skill":
                    this.showSkill();
                    break;
                case "weapon":
                    this.showWeapon();
                    break;
                case "email":
                    this.showEmail(1);
                    break;

                case "activityQuest1":
                    this.showMyQuest(1);
                    break;

                case "activityQuest2":
                    this.showMyQuest(2);
                    break;

                case "activityQuest3":
                    this.showMyQuest(3);
                    break;

                case "activityQuest4":
                    this.showMyQuest(4);
                    break;

                case "historyQuest":
                    this.showMyHistoryQuest();
                    break;
            }
        },
        showWare: function (id) {
            connection.invoke("ShowWare", { myWareId: id });
        },
        closeDetail: function () {
            that.myDetail = "";
        },
        setRoom: function (direction) {
            console.log(direction);
        },
        npcAction: function (npcId, action) {
            console.log("npcId=" + npcId + ",action=" + action);
            connection.invoke("NpcAction",  {  npcId,  action});
        },
        playerAction: function (targetId, command) {
            var that = this;
            console.log("targetId=" + targetId + ",command=" + JSON.stringify(command));
            if (command.tips) {
                that.confirm(command.tips, function () {
                    connection.invoke("PlayerAction", { targetId, commandName: command.commandName });
                });
            }else{
                connection.invoke("PlayerAction", { targetId, commandName:command.commandName });
            }


            
        },
        clickCommand: function (e) {
            var that = this;
            var obj = e.target;
            var action = {  };
            var npcId = obj.getAttribute('npcId')*1;
            action.scriptId = obj.getAttribute('scriptId')*1;
            action.commandId = obj.getAttribute('commandId')*1;
            var questId = obj.getAttribute('questId') * 1;
            console.log("TakeQuest=" + obj.className);

            if (obj.className === 'chat') {
                action.name = "";
                connection.invoke("NpcAction", { npcId, action });
                //console.log("npcId=" + npcId + ",action=" + JSON.stringify(action));

            } else if (obj.className === 'input') {
                action.message = obj.previousElementSibling.value;
                //console.log("npcId=" + npcId + ",action=" + JSON.stringify(action));
                connection.invoke("NpcAction", { npcId, action });
                obj.previousElementSibling.value = "";
            } else if (obj.className === 'quest') {

                console.log("TakeQuest=" + questId);
                connection.invoke("TakeQuest", { questId: questId });
            } else if (obj.className === 'completeQuest') {
                console.log("CompleteQuest=" + questId);
                connection.invoke("CompleteQuest", { questId: questId });
            }
        },
        showMainQuest: function () {
            var that = this;
            that.dialog(that.mainQuest.name, that.mainQuest.description, function () {

            });
        },
        showTips: function (title, content) {
            var that = this;
            that.dialog(title, content, function () {

            });
        },
        load: function (id) {
            console.log("Load=" + id);
            connection.invoke("Load", { myWareId: id });
        },
        unload: function (id) {
            console.log("UnLoad=" + id);
            connection.invoke("UnLoad", { myWareId: id });
        },
        drop: function (name, id) {
            console.log("drop=" + id);
            var that = this;
            that.confirm("要丢掉[" + name + "]吗？", function () {
                connection.invoke("Drop", { myWareId: id });
            });
        },
        deleteEmail: function (id, title) {
            console.log("deleteEmail=" + id);
            var that = this;
            that.confirm("要删除邮件[" + title + "]吗？", function () {
                connection.invoke("DeleteEmail", { playerEmailId: id });
            }); 
        },
        readEmail: function (id, status) {
            console.log("readEmail=" + id + "," + status);

            if (status === 0) {
                connection.invoke("ReadEmail", { playerEmailId: id });
            };
        },
        showHelp: function () {
            var that = this;

            that.myBox = "help";
        },
        showRanking: function () {
            var that = this;

            that.myBox = "ranking";
        },
        showShop: function () {
            var that = this;

            that.myBox = "shop";
        },
        showFriend: function () {
            var that = this;
            connection.invoke("ShowFriend");
           // that.myBox = "friend";
        },
        agreeFriend: function (id, name) {
            var that = this;
            that.confirm("要同意添加[" + name + "]为好友吗？", function () {
                connection.invoke("AgreeFriend", { relationId: id });
                console.log("AgreeFriend=" + id);
            }, function () {
                    connection.invoke("RefuseFriend", { relationId: id });
                    console.log("RefuseFriend=" + id);
            });
        },
        showMyFighting: function () {
            var that = this;
            that.myBox = "fighting";
        },
        showMyKilling: function () {

        }
    },
    watch: {
        myBox(val) {
            var that = this;
            console.log("val=" + val);
            that.myBoxMenus = that.getMenus(val);
            var height = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
            height = height - that.$refs.message.offsetHeight - 52;
            that.$refs.myBox.style.height = height + "px";
            height = height - that.$refs.menu.offsetHeight - 10;
            that.$refs.content.style.height = height + "px";
            that.myDetail = "";
        },
        myDetail(val) {
            
            var that = this;
            console.log("val=" + val);
            var height = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
            height = height - that.$refs.message.offsetHeight - 52;
            that.$refs.myDetail.style.height = height + "px";
            that.$refs.detail && (that.$refs.detail.style.height = height + "px");
            
        },
        chats: {
            handler() {
                var that = this;
                that.$nextTick(function () {
                    if (that.chats.length > 100) {
                        that.chats.shift();
                    }
                    document.querySelector('#chat').scrollTop = document.getElementById('chat').scrollHeight + 30;
                });
            }
        },
        messages: {
            handler() {
                var that = this;
                that.$nextTick(function () {
                    if (that.messages.length > 100) {
                        that.messages.shift();
                    }
                    document.querySelector('#message').scrollTop = document.getElementById('message').scrollHeight + 30;
                });
            }
        }
    },
    mounted: function () {
        this.joinGame();
        clearInterval(this.timer);
        this.ping();
    },
    beforeDestroy() {
        clearInterval(this.timer);
    }
});

