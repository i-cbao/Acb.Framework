(window.webpackJsonp=window.webpackJsonp||[]).push([["chunk-2044"],{"4OzJ":function(t,e,o){"use strict";var a=o("xgnT");o.n(a).a},"CO/P":function(t,e,o){"use strict";o.r(e);var a={name:"EditForm",components:{ArticleDetail:o("EXL7").a}},r=o("KHd+"),s=Object(r.a)(a,function(){var t=this.$createElement;return(this._self._c||t)("article-detail",{attrs:{"is-edit":!0}})},[],!1,null,null,null);s.options.__file="edit.vue";e.default=s.exports},EXL7:function(t,e,o){"use strict";var a=o("P2sY"),r=o.n(a),s=o("glbJ"),n=o("4d7F"),i=o.n(n),l=o("t3Un");var c={name:"SingleImageUpload3",props:{value:{type:String,default:""}},data:function(){return{tempUrl:"",dataObj:{token:"",key:""}}},computed:{imageUrl:function(){return this.value}},methods:{rmImage:function(){this.emitInput("")},emitInput:function(t){this.$emit("input",t)},handleImageSuccess:function(t){this.emitInput(t.files.file)},beforeUpload:function(){var t=this,e=this;return new i.a(function(o,a){Object(l.a)({url:"/qiniu/upload/token",method:"get"}).then(function(a){var r=a.data.qiniu_key,s=a.data.qiniu_token;e._data.dataObj.token=s,e._data.dataObj.key=r,t.tempUrl=a.data.qiniu_url,o(!0)}).catch(function(t){console.log(t),a(!1)})})}}},u=(o("4OzJ"),o("KHd+")),m=Object(u.a)(c,function(){var t=this,e=t.$createElement,o=t._self._c||e;return o("div",{staticClass:"upload-container"},[o("el-upload",{staticClass:"image-uploader",attrs:{data:t.dataObj,multiple:!1,"show-file-list":!1,"on-success":t.handleImageSuccess,drag:"",action:"https://httpbin.org/post"}},[o("i",{staticClass:"el-icon-upload"}),t._v(" "),o("div",{staticClass:"el-upload__text"},[t._v("将文件拖到此处，或"),o("em",[t._v("点击上传")])])]),t._v(" "),o("div",{staticClass:"image-preview image-app-preview"},[o("div",{directives:[{name:"show",rawName:"v-show",value:t.imageUrl.length>1,expression:"imageUrl.length>1"}],staticClass:"image-preview-wrapper"},[o("img",{attrs:{src:t.imageUrl}}),t._v(" "),o("div",{staticClass:"image-preview-action"},[o("i",{staticClass:"el-icon-delete",on:{click:t.rmImage}})])])]),t._v(" "),o("div",{staticClass:"image-preview"},[o("div",{directives:[{name:"show",rawName:"v-show",value:t.imageUrl.length>1,expression:"imageUrl.length>1"}],staticClass:"image-preview-wrapper"},[o("img",{attrs:{src:t.imageUrl}}),t._v(" "),o("div",{staticClass:"image-preview-action"},[o("i",{staticClass:"el-icon-delete",on:{click:t.rmImage}})])])])],1)},[],!1,null,"4f8048f4",null);m.options.__file="singleImage3.vue";var p=m.exports,d=o("Grqa"),f=o("uARZ"),v=o("Yfch"),h=o("JCNI");var g=Object(u.a)({},function(){this.$createElement;this._self._c;return this._m(0)},[function(){var t=this.$createElement,e=this._self._c||t;return e("p",{staticClass:"warn-content"},[this._v("\n  创建和编辑页面是不能被keep-alive 缓存的，因为keep-alive 的include 目前不支持根据路由来缓存，所以目前都是基于component name 来缓存的，如果你想要实现缓存的效果，可以使用localstorage 等浏览器缓存方案。或者不要使用keep-alive\n  的include，直接缓存所有页面。详情见\n  "),e("a",{attrs:{href:"https://panjiachen.github.io/vue-element-admin-site/guide/essentials/tags-view.html",target:"_blank"}},[this._v("文档")])])}],!1,null,null,null);g.options.__file="Warning.vue";var _=g.exports,b={props:{value:{type:Boolean,default:!1}},computed:{comment_disabled:{get:function(){return this.value},set:function(t){this.$emit("input",t)}}}},w=Object(u.a)(b,function(){var t=this,e=t.$createElement,o=t._self._c||e;return o("el-dropdown",{attrs:{"show-timeout":100,trigger:"click"}},[o("el-button",{attrs:{plain:""}},[t._v(t._s(t.comment_disabled?"评论已关闭":"评论已打开")+"\n    "),o("i",{staticClass:"el-icon-caret-bottom el-icon--right"})]),t._v(" "),o("el-dropdown-menu",{staticClass:"no-padding",attrs:{slot:"dropdown"},slot:"dropdown"},[o("el-dropdown-item",[o("el-radio-group",{staticStyle:{padding:"10px"},model:{value:t.comment_disabled,callback:function(e){t.comment_disabled=e},expression:"comment_disabled"}},[o("el-radio",{attrs:{label:!0}},[t._v("关闭评论")]),t._v(" "),o("el-radio",{attrs:{label:!1}},[t._v("打开评论")])],1)],1)],1)],1)},[],!1,null,null,null);w.options.__file="Comment.vue";var F=w.exports,x={props:{value:{required:!0,default:function(){return[]},type:Array}},data:function(){return{platformsOptions:[{key:"a-platform",name:"a-platform"},{key:"b-platform",name:"b-platform"},{key:"c-platform",name:"c-platform"}]}},computed:{platforms:{get:function(){return this.value},set:function(t){this.$emit("input",t)}}}},y=Object(u.a)(x,function(){var t=this,e=t.$createElement,o=t._self._c||e;return o("el-dropdown",{attrs:{"hide-on-click":!1,"show-timeout":100,trigger:"click"}},[o("el-button",{attrs:{plain:""}},[t._v("\n    平台("+t._s(t.platforms.length)+")\n    "),o("i",{staticClass:"el-icon-caret-bottom el-icon--right"})]),t._v(" "),o("el-dropdown-menu",{staticClass:"no-border",attrs:{slot:"dropdown"},slot:"dropdown"},[o("el-checkbox-group",{staticStyle:{padding:"5px 15px"},model:{value:t.platforms,callback:function(e){t.platforms=e},expression:"platforms"}},t._l(t.platformsOptions,function(e){return o("el-checkbox",{key:e.key,attrs:{label:e.key}},[t._v("\n        "+t._s(e.name)+"\n      ")])}))],1)],1)},[],!1,null,null,null);y.options.__file="Platform.vue";var k=y.exports,C={props:{value:{type:String,default:""}},computed:{source_uri:{get:function(){return this.value},set:function(t){this.$emit("input",t)}}}},$=Object(u.a)(C,function(){var t=this,e=t.$createElement,o=t._self._c||e;return o("el-dropdown",{attrs:{"show-timeout":100,trigger:"click"}},[o("el-button",{attrs:{plain:""}},[t._v("\n    外链\n    "),o("i",{staticClass:"el-icon-caret-bottom el-icon--right"})]),t._v(" "),o("el-dropdown-menu",{staticClass:"no-padding no-border",staticStyle:{width:"400px"},attrs:{slot:"dropdown"},slot:"dropdown"},[o("el-form-item",{staticStyle:{"margin-bottom":"0px"},attrs:{"label-width":"0px",prop:"source_uri"}},[o("el-input",{attrs:{placeholder:"请输入内容"},model:{value:t.source_uri,callback:function(e){t.source_uri=e},expression:"source_uri"}},[o("template",{slot:"prepend"},[t._v("填写url")])],2)],1)],1)],1)},[],!1,null,null,null);$.options.__file="SourceUrl.vue";var O=$.exports,S={status:"draft",title:"",content:"",content_short:"",source_uri:"",image_uri:"",display_time:void 0,id:void 0,platforms:["a-platform"],comment_disabled:!1,importance:0},j={name:"ArticleDetail",components:{Tinymce:s.a,MDinput:d.a,Upload:p,Sticky:f.a,Warning:_,CommentDropdown:F,PlatformDropdown:k,SourceUrlDropdown:O},props:{isEdit:{type:Boolean,default:!1}},data:function(){var t=this,e=function(e,o,a){""===o?(t.$message({message:e.field+"为必传项",type:"error"}),a(new Error(e.field+"为必传项"))):a()};return{postForm:r()({},S),loading:!1,userListOptions:[],rules:{image_uri:[{validator:e}],title:[{validator:e}],content:[{validator:e}],source_uri:[{validator:function(e,o,a){o?Object(v.b)(o)?a():(t.$message({message:"外链url填写不正确",type:"error"}),a(new Error("外链url填写不正确"))):a()},trigger:"blur"}]}}},computed:{contentShortLength:function(){return this.postForm.content_short.length}},created:function(){if(this.isEdit){var t=this.$route.params&&this.$route.params.id;this.fetchData(t)}else this.postForm=r()({},S)},methods:{fetchData:function(t){var e=this;Object(h.b)(t).then(function(t){e.postForm=t.data,e.postForm.title+="   Article Id:"+e.postForm.id,e.postForm.content_short+="   Article Id:"+e.postForm.id}).catch(function(t){console.log(t)})},submitForm:function(){var t=this;this.postForm.display_time=parseInt(this.display_time/1e3),console.log(this.postForm),this.$refs.postForm.validate(function(e){if(!e)return console.log("error submit!!"),!1;t.loading=!0,t.$notify({title:"成功",message:"发布文章成功",type:"success",duration:2e3}),t.postForm.status="published",t.loading=!1})},draftForm:function(){0!==this.postForm.content.length&&0!==this.postForm.title.length?(this.$message({message:"保存成功",type:"success",showClose:!0,duration:1e3}),this.postForm.status="draft"):this.$message({message:"请填写必要的标题和内容",type:"warning"})},getRemoteUserList:function(t){var e=this;(function(t){return Object(l.a)({url:"/search/user",method:"get",params:{name:t}})})(t).then(function(t){t.data.items&&(e.userListOptions=t.data.items.map(function(t){return t.name}))})}}},U=(o("NL/9"),Object(u.a)(j,function(){var t=this,e=t.$createElement,o=t._self._c||e;return o("div",{staticClass:"createPost-container"},[o("el-form",{ref:"postForm",staticClass:"form-container",attrs:{model:t.postForm,rules:t.rules}},[o("sticky",{attrs:{"class-name":"sub-navbar "+t.postForm.status}},[o("CommentDropdown",{model:{value:t.postForm.comment_disabled,callback:function(e){t.$set(t.postForm,"comment_disabled",e)},expression:"postForm.comment_disabled"}}),t._v(" "),o("PlatformDropdown",{model:{value:t.postForm.platforms,callback:function(e){t.$set(t.postForm,"platforms",e)},expression:"postForm.platforms"}}),t._v(" "),o("SourceUrlDropdown",{model:{value:t.postForm.source_uri,callback:function(e){t.$set(t.postForm,"source_uri",e)},expression:"postForm.source_uri"}}),t._v(" "),o("el-button",{directives:[{name:"loading",rawName:"v-loading",value:t.loading,expression:"loading"}],staticStyle:{"margin-left":"10px"},attrs:{type:"success"},on:{click:t.submitForm}},[t._v("发布\n      ")]),t._v(" "),o("el-button",{directives:[{name:"loading",rawName:"v-loading",value:t.loading,expression:"loading"}],attrs:{type:"warning"},on:{click:t.draftForm}},[t._v("草稿")])],1),t._v(" "),o("div",{staticClass:"createPost-main-container"},[o("el-row",[o("Warning"),t._v(" "),o("el-col",{attrs:{span:24}},[o("el-form-item",{staticStyle:{"margin-bottom":"40px"},attrs:{prop:"title"}},[o("MDinput",{attrs:{maxlength:100,name:"name",required:""},model:{value:t.postForm.title,callback:function(e){t.$set(t.postForm,"title",e)},expression:"postForm.title"}},[t._v("\n              标题\n            ")])],1),t._v(" "),o("div",{staticClass:"postInfo-container"},[o("el-row",[o("el-col",{attrs:{span:8}},[o("el-form-item",{staticClass:"postInfo-container-item",attrs:{"label-width":"45px",label:"作者:"}},[o("el-select",{attrs:{"remote-method":t.getRemoteUserList,filterable:"",remote:"",placeholder:"搜索用户"},model:{value:t.postForm.author,callback:function(e){t.$set(t.postForm,"author",e)},expression:"postForm.author"}},t._l(t.userListOptions,function(t,e){return o("el-option",{key:t+e,attrs:{label:t,value:t}})}))],1)],1),t._v(" "),o("el-col",{attrs:{span:10}},[o("el-form-item",{staticClass:"postInfo-container-item",attrs:{"label-width":"80px",label:"发布时间:"}},[o("el-date-picker",{attrs:{type:"datetime",format:"yyyy-MM-dd HH:mm:ss",placeholder:"选择日期时间"},model:{value:t.postForm.display_time,callback:function(e){t.$set(t.postForm,"display_time",e)},expression:"postForm.display_time"}})],1)],1),t._v(" "),o("el-col",{attrs:{span:6}},[o("el-form-item",{staticClass:"postInfo-container-item",attrs:{"label-width":"60px",label:"重要性:"}},[o("el-rate",{staticStyle:{"margin-top":"8px"},attrs:{max:3,colors:["#99A9BF","#F7BA2A","#FF9900"],"low-threshold":1,"high-threshold":3},model:{value:t.postForm.importance,callback:function(e){t.$set(t.postForm,"importance",e)},expression:"postForm.importance"}})],1)],1)],1)],1)],1)],1),t._v(" "),o("el-form-item",{staticStyle:{"margin-bottom":"40px"},attrs:{"label-width":"45px",label:"摘要:"}},[o("el-input",{staticClass:"article-textarea",attrs:{rows:1,type:"textarea",autosize:"",placeholder:"请输入内容"},model:{value:t.postForm.content_short,callback:function(e){t.$set(t.postForm,"content_short",e)},expression:"postForm.content_short"}}),t._v(" "),o("span",{directives:[{name:"show",rawName:"v-show",value:t.contentShortLength,expression:"contentShortLength"}],staticClass:"word-counter"},[t._v(t._s(t.contentShortLength)+"字")])],1),t._v(" "),o("div",{staticClass:"editor-container"},[o("Tinymce",{ref:"editor",attrs:{height:400},model:{value:t.postForm.content,callback:function(e){t.$set(t.postForm,"content",e)},expression:"postForm.content"}})],1),t._v(" "),o("div",{staticStyle:{"margin-bottom":"20px"}},[o("Upload",{model:{value:t.postForm.image_uri,callback:function(e){t.$set(t.postForm,"image_uri",e)},expression:"postForm.image_uri"}})],1)],1)],1)],1)},[],!1,null,"33ffb115",null));U.options.__file="ArticleDetail.vue";e.a=U.exports},JCNI:function(t,e,o){"use strict";o.d(e,"c",function(){return r}),o.d(e,"b",function(){return s}),o.d(e,"d",function(){return n}),o.d(e,"a",function(){return i}),o.d(e,"e",function(){return l});var a=o("t3Un");function r(t){return Object(a.a)({url:"/article/list",method:"get",params:t})}function s(t){return Object(a.a)({url:"/article/detail",method:"get",params:{id:t}})}function n(t){return Object(a.a)({url:"/article/pv",method:"get",params:{pv:t}})}function i(t){return Object(a.a)({url:"/article/create",method:"post",data:t})}function l(t){return Object(a.a)({url:"/article/update",method:"post",data:t})}},"NL/9":function(t,e,o){"use strict";var a=o("cFC/");o.n(a).a},"cFC/":function(t,e,o){},xgnT:function(t,e,o){}}]);