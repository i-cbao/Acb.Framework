webpackJsonp([1],{EtAD:function(e,t,n){var o={"./Home.vue":"j7e0"};function i(e){return n(s(e))}function s(e){var t=o[e];if(!(t+1))throw new Error("Cannot find module '"+e+"'.");return t}i.keys=function(){return Object.keys(o)},i.resolve=s,e.exports=i,i.id="EtAD"},NHnr:function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var o=n("7+uW"),i={render:function(){var e=this.$createElement,t=this._self._c||e;return t("div",{attrs:{id:"app"}},[t("router-view")],1)},staticRenderFns:[]},s=n("VU/8")({name:"App"},i,!1,null,null,null).exports,a=n("/ocq");o.default.use(a.a);var r,c=new a.a({routes:[{path:"/",name:"Home",component:(r="Home",n("EtAD")("./"+r+".vue").default)}]}),l=n("zL8q"),u=n.n(l);n("tvR6");o.default.config.productionTip=!1,o.default.use(u.a),new o.default({el:"#app",router:c,components:{App:s},template:"<App/>"})},j7e0:function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var o=n("//Fk"),i=n.n(o),s=n("mtWM"),a=n.n(s),r=n("lbHh"),c=n.n(r),l="__acb_config_ticket";function u(){return c.a.get(l)}function f(e){c.a.set(l,e,{expires:7})}var d=a.a.create({baseURL:"",timeout:3e5});d.interceptors.request.use(function(e){var t=u();return e.headers.Authorization=t?"acb "+t:" basic config",e},function(e){return i.a.reject({status:-1,message:e.message})}),d.interceptors.response.use(function(e){var t=e.data;return t.hasOwnProperty("ok")&&!t.ok?i.a.reject({status:-1,message:t.message}):t},function(e){return i.a.reject({status:-1,message:e.message})});var g=d,h=function(e,t){return"default"===t?e:e+"/"+t},m=n("pFYg"),p=n.n(m);var v={name:"Home",data:function(){return{isLoading:!1,dialogVisible:!1,form:{account:"",password:""},list:[],envs:["default","dev","test","ready","prod"],modes:["text","code","tree","view","form"],configMode:"view",configEnv:"default",configKey:"",config:"",editor:!1}},created:function(){this.loadList()},mounted:function(){this.editor=new JSONEditor(document.getElementById("jsoneditor"),{mode:this.configMode,search:!0,height:500})},computed:{saveStatus:function(){return this.configKey&&this.configKey.length>1},deleteStatus:function(){return this.configKey&&this.configKey.length>1}},methods:{checkLogin:function(e){e.message.indexOf("403")>=0&&(this.dialogVisible=!0)},loadList:function(){var e=this;g.get("list").then(function(t){e.list=t}).catch(this.checkLogin)},loadConfig:function(){var e,t,n=this;(e=this.configKey,t=this.configEnv,g.get(e+"/"+t)).then(function(e){n.editor.set(e||{}),"tree"!==n.configMode&&"view"!==n.configMode&&"form"!==n.configMode||n.editor.expandAll()}).catch(this.checkLogin)},selectKey:function(e){this.configKey=e,this.loadConfig()},envChange:function(e){this.loadConfig()},modeChange:function(){this.editor.setMode(this.configMode)},save:function(){var e,t,n,o=this;if(this.config=this.editor.getText(),!function(e){try{if("object"==p()(JSON.parse(e)))return!0}catch(e){}return!1}(this.config))return this.$message({message:"配置信息必须为Json格式",type:"warning"}),!1;this.isLoading=!0,(e=this.configKey,t=this.configEnv,n=this.config,g.post(h(e,t),{config:n})).then(function(e){e&&e.ok&&(o.$message({message:"配置文件保存成功",type:"success"}),o.loadList(),o.isLoading=!1)}).catch(function(){o.isLoading=!1})},remove:function(){var e=this;this.$confirm("此操作将删除该配置文件, 是否继续?","提示",{confirmButtonText:"确定",cancelButtonText:"取消",type:"warning"}).then(function(){var t,n;(t=e.configKey,n=e.configEnv,g.delete(h(t,n))).then(function(t){t&&t.ok&&(e.$message({message:"配置文件删除成功",type:"success"}),e.loadList(),e.loadConfig())})})},handleLogin:function(){var e,t,n=this;if(!this.form.account||!this.form.password)return this.$message({message:"请输入账号密码",type:"error"}),!1;(e=this.form.account,t=this.form.password,g.post("login",{account:e,password:t}).then(function(e){f(e.ticket)})).then(function(e){location.reload(!0)}).catch(function(e){n.$message({message:e.message,type:"error"})})},handleClose:function(){return!1}}},y={render:function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("el-container",[n("el-header",[e._v("配置中心")]),e._v(" "),n("el-container",{attrs:{loading:e.isLoading}},[n("el-aside",{attrs:{width:"220px"}},[n("el-menu",{on:{select:e.selectKey}},e._l(e.list,function(t){return n("el-menu-item",{key:t,attrs:{index:t}},[e._v(e._s(t))])}))],1),e._v(" "),n("el-main",[n("el-row",{attrs:{gutter:20}},[n("el-col",{attrs:{span:6}},[n("el-input",{attrs:{type:"text",placeholder:"请输入配置项"},model:{value:e.configKey,callback:function(t){e.configKey=t},expression:"configKey"}})],1),e._v(" "),n("el-col",{attrs:{span:4}},[n("el-select",{attrs:{placeholder:"请选择环境"},on:{change:e.envChange},model:{value:e.configEnv,callback:function(t){e.configEnv=t},expression:"configEnv"}},e._l(e.envs,function(e){return n("el-option",{key:e,attrs:{label:e,value:e}})}))],1),e._v(" "),n("el-col",{attrs:{span:4}},[n("el-select",{attrs:{placeholder:"请选择编辑模式"},on:{change:e.modeChange},model:{value:e.configMode,callback:function(t){e.configMode=t},expression:"configMode"}},e._l(e.modes,function(e){return n("el-option",{key:e,attrs:{label:e,value:e}})}))],1)],1),e._v(" "),n("el-row",[n("el-col",{attrs:{span:16}},[n("div",{attrs:{id:"jsoneditor"}})])],1),e._v(" "),n("el-row",[n("el-button",{attrs:{type:"primary",disabled:!e.saveStatus},on:{click:e.save}},[e._v("保存配置")]),e._v(" "),n("el-button",{attrs:{type:"danger",disabled:!e.saveStatus},on:{click:e.remove}},[e._v("删除配置")])],1)],1),e._v(" "),n("el-dialog",{attrs:{title:"用户登录",visible:e.dialogVisible,width:"400px","show-close":!1,"before-close":e.handleClose},on:{"update:visible":function(t){e.dialogVisible=t}}},[n("el-form",{ref:"form",attrs:{model:e.form,"label-width":"80px"}},[n("el-form-item",{attrs:{label:"登录账号"}},[n("el-input",{nativeOn:{keyup:function(t){return"button"in t||!e._k(t.keyCode,"enter",13,t.key,"Enter")?e.handleLogin(t):null}},model:{value:e.form.account,callback:function(t){e.$set(e.form,"account",t)},expression:"form.account"}})],1),e._v(" "),n("el-form-item",{attrs:{label:"登录密码"}},[n("el-input",{attrs:{type:"password"},nativeOn:{keyup:function(t){return"button"in t||!e._k(t.keyCode,"enter",13,t.key,"Enter")?e.handleLogin(t):null}},model:{value:e.form.password,callback:function(t){e.$set(e.form,"password",t)},expression:"form.password"}})],1),e._v(" "),n("el-form-item",[n("el-button",{attrs:{type:"primary"},on:{click:e.handleLogin}},[e._v("立即登录")])],1)],1)],1)],1)],1)},staticRenderFns:[]};var b=n("VU/8")(v,y,!1,function(e){n("pas/")},null,null);t.default=b.exports},"pas/":function(e,t){},tvR6:function(e,t){}},["NHnr"]);
//# sourceMappingURL=app.082d0b44134841f7e1f4.js.map