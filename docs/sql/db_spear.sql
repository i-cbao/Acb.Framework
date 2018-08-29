/*
 Navicat Premium Data Transfer

 Source Server         : 192.168.0.251
 Source Server Type    : PostgreSQL
 Source Server Version : 100004
 Source Host           : 192.168.0.251:15432
 Source Catalog        : db_spear
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 100004
 File Encoding         : 65001

 Date: 29/08/2018 18:49:03
*/


-- ----------------------------
-- Table structure for t_config
-- ----------------------------
DROP TABLE IF EXISTS "t_config";
CREATE TABLE "t_config" (
  "Id" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "Name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "Mode" varchar(16) COLLATE "pg_catalog"."default" DEFAULT 0,
  "Content" text COLLATE "pg_catalog"."default" NOT NULL,
  "ProjectCode" varchar(32) COLLATE "pg_catalog"."default" NOT NULL DEFAULT ''::character varying,
  "Md5" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "Desc" varchar(128) COLLATE "pg_catalog"."default",
  "Status" int2 NOT NULL,
  "Timestamp" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "t_config"."Name" IS '名称';
COMMENT ON COLUMN "t_config"."Mode" IS '模式:Dev,Test,Ready,Prod...';
COMMENT ON COLUMN "t_config"."Content" IS '内容';
COMMENT ON COLUMN "t_config"."ProjectCode" IS '项目编码';
COMMENT ON COLUMN "t_config"."Md5" IS '配置校验码';
COMMENT ON COLUMN "t_config"."Desc" IS '描述';
COMMENT ON COLUMN "t_config"."Status" IS '状态:0,正常;1,历史版本;2,已删除';

-- ----------------------------
-- Records of t_config
-- ----------------------------
BEGIN;
INSERT INTO "t_config" VALUES ('1e10cd93374dc463514e08d5fbc5eeda', 'dapper', 'dev', '{"dapper":{"default":{"ConnectionString":"server=140.143.96.228;user=root;database=icbv2db;port=3306;password=icb@888;Pooling=true;Charset=utf8;","Name":"default","ProviderName":"mysql"},"icb_main":{"ConnectionString":"server=140.143.96.228;user=root;database=db_main;port=3306;password=icb@888;Pooling=true;Charset=utf8;","Name":"icb_main","ProviderName":"mysql"},"statistic":{"ConnectionString":"server=140.143.96.228;user=root;database=icb_statistics_db;port=3306;password=icb@888;Pooling=true;","Name":"statistic","ProviderName":"mysql"},"sword":{"ConnectionString":"server=140.143.96.228;user=root;database=db_sword;port=3306;password=icb@888;Pooling=true;Charset=utf8;","Name":"sword","ProviderName":"mysql"},"shield":{"ConnectionString":"server=140.143.96.228;user=root;database=db_shield;port=3306;password=icb@888;Pooling=true;Charset=utf8;","Name":"shield","ProviderName":"mysql"}}}', 'icb', '18BDB5F3CC905E6F052711E4775DC08C', NULL, 0, '2018-08-06 17:56:28');
INSERT INTO "t_config" VALUES ('cfd6ab9b4210c97abf0c08d5fbc5c82b', 'redis', 'dev', '{"redis":{"default":"140.143.96.228:6379,password=icb@888,abortConnect=false,connectRetry=3,connectTimeout=3000,defaultDatabase=0,syncTimeout=3000,version=3.2.1,responseTimeout=3000","eventBus":"140.143.96.228:6379,password=icb@888,abortConnect=false,connectRetry=3,connectTimeout=3000,defaultDatabase=0,syncTimeout=3000,version=3.2.1,responseTimeout=3000","sync":"140.143.96.228:6379,password=icb@888,abortConnect=false,connectRetry=3,connectTimeout=3000,defaultDatabase=0,syncTimeout=3000,version=3.2.1,responseTimeout=3000"}}', 'icb', '4763E59223AC0083D549A542F52E4ABD', NULL, 0, '2018-08-06 17:55:23');
INSERT INTO "t_config" VALUES ('fbaef876b990cdaf6a3b08d5fbb9ec12', 'basic', 'dev', '{"mode":"Dev","logLevel":"Debug"}', 'icb', '7B79378A8B0CAE2EA550F6F4046878B4', NULL, 1, '2018-08-06 16:30:29');
INSERT INTO "t_config" VALUES ('46e62a8a1128cb9de88e08d5fbb62ef5', 'basic', 'dev', '{"mode":"Dev"}', 'icb', '4A68E466F6167DFC89E04BE13FE15F14', NULL, 1, '2018-08-06 16:03:43');
INSERT INTO "t_config" VALUES ('6387cc8a42f1cb6bc39a08d5fbb61329', 'basic', NULL, '{"mode":"Test"}', 'icb', '9C1F0EB329A54AE138E3F99E58D231C9', NULL, 0, '2018-08-06 16:02:57');
INSERT INTO "t_config" VALUES ('192223afd1efcf6545cc08d5fbb80d84', 'basic', 'prod', '{"mode":"Prod"}', 'icb', 'D64C86C10A5FE7EF6996BF5F436A3B92', NULL, 0, '2018-08-06 16:17:06');
INSERT INTO "t_config" VALUES ('fc35c867bdb0cb15885208d5fbc5f595', 'mongo', 'dev', '{"mongo":{"default":{"credentials":[{"database":"admin","pwd":"icb@888","user":"root"}],"servers":[{"host":"140.143.96.228","port":13033}],"timeout":5000},"web":{"credentials":[{"database":"admin","pwd":"icb@888","user":"root"}],"servers":[{"host":"140.143.96.228","port":13033}],"timeout":5000}}}', 'icb', 'BB7AE1C865E8F9DE8BF345D0E66A4E01', NULL, 0, '2018-08-06 17:56:39');
INSERT INTO "t_config" VALUES ('32e19ef519dbc92d3c2808d60ddd2c89', 'rabbit', 'dev', '{"rabbit":{"default":{"host":"192.168.0.251","port":5672,"user":"icb","password":"a123456","broker":"icb_broker","virtualHost":"icb"}}}', 'icb', '6F804A21C415DD05BE8A853255CB6A9A', NULL, 0, '2018-08-29 18:28:11');
INSERT INTO "t_config" VALUES ('7ad2a158c01ec6b04b2908d60d15e9b0', 'basic', 'dev', '{"actionTimingThreshold":200,"app_sign_key":"","commonTicket":"1484356604d8762b16d360fda70417345bbae013e2","const":{"icb_id":"1514814a05534c1cb1a1147a13e0cbad","mongo_db":"DBV3","report_limit":5,"gps_signal_time":300},"micro_service":{"register":"consul","consulServer":"http://192.168.0.252:8500","consulCheck":false,"consulInterval":30,"deregisterAfter":180},"sites":{"app":"http://192.168.0.252:6301","file":"http://140.143.96.228:8209","gps":"http://192.168.0.216:9099","management":"http://192.168.0.252:6302","open":"http://140.143.96.228:8206","static":"http://static.i-cbao.com","v2Api":"http://192.168.0.252:6101","risk":"http://192.168.0.252:2022"},"tcpLogger":{"address":"192.168.0.251","layout":"[%date] [%-5level] [%property{LogSite}] %r %thread [%logger] ##%message## ##%exception#_#","level":"Error","port":8610},"ticketKey":"mrjjv1apqhtfuswmah0cgvxngi8yuumg"}', 'icb', '2BB918207020E24B45F0322D868A36F2', NULL, 0, '2018-08-28 18:41:49');
INSERT INTO "t_config" VALUES ('2bb3d661e985c7c3c5f708d60ddedb7f', 'sites', 'test', '{"file":"http://file.i-cbao.com"}', 'icbhs', '1EAEB1938B8F7EA3AB3E23FF11828629', NULL, 0, '2018-08-29 18:40:14');
COMMIT;

-- ----------------------------
-- Table structure for t_config_project
-- ----------------------------
DROP TABLE IF EXISTS "t_config_project";
CREATE TABLE "t_config_project" (
  "Id" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "Name" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "Security" int2 NOT NULL,
  "Account" varchar(64) COLLATE "pg_catalog"."default",
  "Password" varchar(128) COLLATE "pg_catalog"."default",
  "Desc" varchar(128) COLLATE "pg_catalog"."default",
  "Code" varchar(16) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "t_config_project"."Name" IS '项目名称';
COMMENT ON COLUMN "t_config_project"."Security" IS '安全性:0,匿名;1,管理验证;2.获取验证;';
COMMENT ON COLUMN "t_config_project"."Account" IS '帐号';
COMMENT ON COLUMN "t_config_project"."Password" IS '密码';
COMMENT ON COLUMN "t_config_project"."Desc" IS '描述';
COMMENT ON COLUMN "t_config_project"."Code" IS '项目编码';

-- ----------------------------
-- Records of t_config_project
-- ----------------------------
BEGIN;
INSERT INTO "t_config_project" VALUES ('5afba2f49077c91c963d08d5fbcacf01', '爱车保', 3, 'icb', 'E10ADC3949BA59ABBE56E057F20F883E', '爱车保项目', 'icb');
INSERT INTO "t_config_project" VALUES ('d3e356f5b686c00ee22408d60ddebb59', 'i车保护神', 2, 'icbhs', 'E10ADC3949BA59ABBE56E057F20F883E', 'i车保护神项目', 'icbhs');
COMMIT;

-- ----------------------------
-- Table structure for t_job
-- ----------------------------
DROP TABLE IF EXISTS "t_job";
CREATE TABLE "t_job" (
  "Id" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "Name" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "Group" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "Status" int4 NOT NULL,
  "Type" int4 NOT NULL,
  "Desc" varchar(512) COLLATE "pg_catalog"."default",
  "CreationTime" timestamp(0)
)
;
COMMENT ON COLUMN "t_job"."Name" IS '任务名称';
COMMENT ON COLUMN "t_job"."Group" IS '任务分组';
COMMENT ON COLUMN "t_job"."Status" IS '任务状态';
COMMENT ON COLUMN "t_job"."Type" IS '任务类型';
COMMENT ON COLUMN "t_job"."Desc" IS '任务描述';
COMMENT ON COLUMN "t_job"."CreationTime" IS '创建时间';

-- ----------------------------
-- Records of t_job
-- ----------------------------
BEGIN;
INSERT INTO "t_job" VALUES ('46d7dca5abe5cf21cf8908d5f9611836', '百度接口', '获取数据', 0, 0, '百度接口测试', '2018-08-03 16:49:36');
COMMIT;

-- ----------------------------
-- Table structure for t_job_http
-- ----------------------------
DROP TABLE IF EXISTS "t_job_http";
CREATE TABLE "t_job_http" (
  "Id" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "Url" varchar(512) COLLATE "pg_catalog"."default" NOT NULL,
  "Method" int4 NOT NULL DEFAULT 0,
  "BodyType" int4 NOT NULL DEFAULT 0,
  "Header" varchar(1024) COLLATE "pg_catalog"."default",
  "Data" text COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "t_job_http"."Url" IS 'Url';
COMMENT ON COLUMN "t_job_http"."Method" IS '请求方式';
COMMENT ON COLUMN "t_job_http"."BodyType" IS '数据类型';
COMMENT ON COLUMN "t_job_http"."Header" IS '请求头';
COMMENT ON COLUMN "t_job_http"."Data" IS '请求数据';

-- ----------------------------
-- Records of t_job_http
-- ----------------------------
BEGIN;
INSERT INTO "t_job_http" VALUES ('46d7dca5abe5cf21cf8908d5f9611836', 'http://www.baidu.com', 0, 0, NULL, NULL);
COMMIT;

-- ----------------------------
-- Table structure for t_job_record
-- ----------------------------
DROP TABLE IF EXISTS "t_job_record";
CREATE TABLE "t_job_record" (
  "Id" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "JobId" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "Status" int4 NOT NULL,
  "StartTime" timestamp(0) NOT NULL,
  "CompleteTime" timestamp(0) NOT NULL,
  "Result" text COLLATE "pg_catalog"."default",
  "Remark" varchar(1024) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "t_job_record"."JobId" IS '任务ID';
COMMENT ON COLUMN "t_job_record"."Status" IS '状态';
COMMENT ON COLUMN "t_job_record"."StartTime" IS '开始时间';
COMMENT ON COLUMN "t_job_record"."CompleteTime" IS '结束时间';
COMMENT ON COLUMN "t_job_record"."Result" IS '执行结果';
COMMENT ON COLUMN "t_job_record"."Remark" IS '备注';

-- ----------------------------
-- Records of t_job_record
-- ----------------------------
BEGIN;
INSERT INTO "t_job_record" VALUES ('215461fa6689c1dfe48c08d5fedb7d86', '46d7dca5abe5cf21cf8908d5f9611836', 1, '2018-08-10 16:08:20', '2018-08-10 16:08:20', 'code:OK,content:<!DOCTYPE html><!--STATUS OK-->
<html>
<head>
	<meta http-equiv="content-type" content="text/html;charset=utf-8">
	<meta http-equiv="X-UA-Compatible" content="IE=Edge">
	<link rel="dns-prefetch" href="//s1.bdstatic.com"/>
	<link rel="dns-prefetch" href="//t1.baidu.com"/>
	<link rel="dns-prefetch" href="//t2.baidu.com"/>
	<link rel="dns-prefetch" href="//t3.baidu.com"/>
	<link rel="dns-prefetch" href="//t10.baidu.com"/>
	<link rel="dns-prefetch" href="//t11.baidu.com"/>
	<link rel="dns-prefetch" href="//t12.baidu.com"/>
	<link rel="dns-prefetch" href="//b1.bdstatic.com"/>
	<title>百度一下，你就知道</title>
	<link href="http://s1.bdstatic.com/r/www/cache/static/home/css/index.css" rel="stylesheet" type="text/css" />
	<!--[if lte IE 8]><style index="index" >#content{height:480px\9}#m{top:260px\9}</style><![endif]-->
	<!--[if IE 8]><style index="index" >#u1 a.mnav,#u1 a.mnav:visited{font-family:simsun}</style><![endif]-->
	<script>var hashMatch = document.location.href.match(/#+(.*wd=[^&].+)/);if (hashMatch && hashMatch[0] && hashMatch[1]) {document.location.replace("http://"+location.host+"/s?"+hashMatch[1]);}var ns_c = function(){};</script>
	<script>function h(obj){obj.style.behavior=''url(#default#homepage)'';var a = obj.setHomePage(''//www.baidu.com/'');}</script>
	<noscript><meta http-equiv="refresh" content="0; url=/baidu.html?from=noscript"/></noscript>
	<script>window._ASYNC_START=new Date().getTime();</script>
</head>
<body link="#0000cc"><div id="wrapper" style="display:none;"><div id="u"><a href="//www.baidu.com/gaoji/preferences.html"  onmousedown="return user_c({''fm'':''set'',''tab'':''setting'',''login'':''0''})">搜索设置</a>|<a id="btop" href="/"  onmousedown="return user_c({''fm'':''set'',''tab'':''index'',''login'':''0''})">百度首页</a>|<a id="lb" href="https://passport.baidu.com/v2/?login&tpl=mn&u=http%3A%2F%2Fwww.baidu.com%2F" onclick="return false;"  onmousedown="return user_c({''fm'':''set'',''tab'':''login''})">登录</a><a href="https://passport.baidu.com/v2/?reg&regType=1&tpl=mn&u=http%3A%2F%2Fwww.baidu.com%2F"  onmousedown="return user_c({''fm'':''set'',''tab'':''reg''})" target="_blank" class="reg">注册</a></div><div id="head"><div class="s_nav"><a href="/" class="s_logo" onmousedown="return c({''fm'':''tab'',''tab'':''logo''})"><img src="//www.baidu.com/img/baidu_jgylogo3.gif" width="117" height="38" border="0" alt="到百度首页" title="到百度首页"></a><div class="s_tab" id="s_tab"><a href="http://news.baidu.com/ns?cl=2&rn=20&tn=news&word=" wdfield="word"  onmousedown="return c({''fm'':''tab'',''tab'':''news''})">新闻</a>&#12288;<b>网页</b>&#12288;<a href="http://tieba.baidu.com/f?kw=&fr=wwwt" wdfield="kw"  onmousedown="return c({''fm'':''tab'',''tab'':''tieba''})">贴吧</a>&#12288;<a href="http://zhidao.baidu.com/q?ct=17&pn=0&tn=ikaslist&rn=10&word=&fr=wwwt" wdfield="word"  onmousedown="return c({''fm'':''tab'',''tab'':''zhidao''})">知道</a>&#12288;<a href="http://music.baidu.com/search?fr=ps&key=" wdfield="key"  onmousedown="return c({''fm'':''tab'',''tab'':''music''})">音乐</a>&#12288;<a href="http://image.baidu.com/i?tn=baiduimage&ps=1&ct=201326592&lm=-1&cl=2&nc=1&word=" wdfield="word"  onmousedown="return c({''fm'':''tab'',''tab'':''pic''})">图片</a>&#12288;<a href="http://v.baidu.com/v?ct=301989888&rn=20&pn=0&db=0&s=25&word=" wdfield="word"   onmousedown="return c({''fm'':''tab'',''tab'':''video''})">视频</a>&#12288;<a href="http://map.baidu.com/m?word=&fr=ps01000" wdfield="word"  onmousedown="return c({''fm'':''tab'',''tab'':''map''})">地图</a>&#12288;<a href="http://wenku.baidu.com/search?word=&lm=0&od=0" wdfield="word"  onmousedown="return c({''fm'':''tab'',''tab'':''wenku''})">文库</a>&#12288;<a href="//www.baidu.com/more/"  onmousedown="return c({''fm'':''tab'',''tab'':''more''})">更多»</a></div></div><form id="form" name="f" action="/s" class="fm" ><input type="hidden" name="ie" value="utf-8"><input type="hidden" name="f" value="8"><input type="hidden" name="rsv_bp" value="1"><span class="bg s_ipt_wr"><input name="wd" id="kw" class="s_ipt" value="" maxlength="100"></span><span class="bg s_btn_wr"><input type="submit" id="su" value="百度一下" class="bg s_btn" onmousedown="this.className=''bg s_btn s_btn_h''" onmouseout="this.className=''bg s_btn''"></span><span class="tools"><span id="mHolder"><div id="mCon"><span>输入法</span></div><ul id="mMenu"><li><a href="javascript:;" name="ime_hw">手写</a></li><li><a href="javascript:;" name="ime_py">拼音</a></li><li class="ln"></li><li><a href="javascript:;" name="ime_cl">关闭</a></li></ul></span><span class="shouji"><strong>推荐&nbsp;:&nbsp;</strong><a href="http://w.x.baidu.com/go/mini/8/10000020" onmousedown="return ns_c({''fm'':''behs'',''tab'':''bdbrowser''})">百度浏览器，打开网页快2秒！</a></span></span></form></div><div id="content"><div id="u1"><a href="http://news.baidu.com" name="tj_trnews" class="mnav">新闻</a><a href="http://www.hao123.com" name="tj_trhao123" class="mnav">hao123</a><a href="http://map.baidu.com" name="tj_trmap" class="mnav">地图</a><a href="http://v.baidu.com" name="tj_trvideo" class="mnav">视频</a><a href="http://tieba.baidu.com" name="tj_trtieba" class="mnav">贴吧</a><a href="https://passport.baidu.com/v2/?login&tpl=mn&u=http%3A%2F%2Fwww.baidu.com%2F" name="tj_login" id="lb" onclick="return false;">登录</a><a href="//www.baidu.com/gaoji/preferences.html" name="tj_settingicon" id="pf">设置</a><a href="//www.baidu.com/more/" name="tj_briicon" id="bri">更多产品</a></div><div id="m"><p id="lg"><img src="//www.baidu.com/img/bd_logo.png" width="270" height="129"></p><p id="nv"><a href="http://news.baidu.com">新&nbsp;闻</a>　<b>网&nbsp;页</b>　<a href="http://tieba.baidu.com">贴&nbsp;吧</a>　<a href="http://zhidao.baidu.com">知&nbsp;道</a>　<a href="http://music.baidu.com">音&nbsp;乐</a>　<a href="http://image.baidu.com">图&nbsp;片</a>　<a href="http://v.baidu.com">视&nbsp;频</a>　<a href="http://map.baidu.com">地&nbsp;图</a></p><div id="fm"><form id="form1" name="f1" action="/s" class="fm"><span class="bg s_ipt_wr"><input type="text" name="wd" id="kw1" maxlength="100" class="s_ipt"></span><input type="hidden" name="rsv_bp" value="0"><input type=hidden name=ch value=""><input type=hidden name=tn value="baidu"><input type=hidden name=bar value=""><input type="hidden" name="rsv_spt" value="3"><input type="hidden" name="ie" value="utf-8"><span class="bg s_btn_wr"><input type="submit" value="百度一下" id="su1" class="bg s_btn" onmousedown="this.className=''bg s_btn s_btn_h''" onmouseout="this.className=''bg s_btn''"></span></form><span class="tools"><span id="mHolder1"><div id="mCon1"><span>输入法</span></div></span></span><ul id="mMenu1"><div class="mMenu1-tip-arrow"><em></em><ins></ins></div><li><a href="javascript:;" name="ime_hw">手写</a></li><li><a href="javascript:;" name="ime_py">拼音</a></li><li class="ln"></li><li><a href="javascript:;" name="ime_cl">关闭</a></li></ul></div><p id="lk"><a href="http://baike.baidu.com">百科</a>　<a href="http://wenku.baidu.com">文库</a>　<a href="http://www.hao123.com">hao123</a><span>&nbsp;|&nbsp;<a href="//www.baidu.com/more/">更多&gt;&gt;</a></span></p><p id="lm"></p></div></div><div id="ftCon"><div id="ftConw"><p id="lh"><a id="seth" onClick="h(this)" href="/" onmousedown="return ns_c({''fm'':''behs'',''tab'':''homepage'',''pos'':0})">把百度设为主页</a><a id="setf" href="//www.baidu.com/cache/sethelp/index.html" onmousedown="return ns_c({''fm'':''behs'',''tab'':''favorites'',''pos'':0})" target="_blank">把百度设为主页</a><a onmousedown="return ns_c({''fm'':''behs'',''tab'':''tj_about''})" href="http://home.baidu.com">关于百度</a><a onmousedown="return ns_c({''fm'':''behs'',''tab'':''tj_about_en''})" href="http://ir.baidu.com">About Baidu</a></p><p id="cp">&copy;2018&nbsp;Baidu&nbsp;<a href="/duty/" name="tj_duty">使用百度前必读</a>&nbsp;京ICP证030173号&nbsp;<img src="http://s1.bdstatic.com/r/www/cache/static/global/img/gs_237f015b.gif"></p></div></div><div id="wrapper_wrapper"></div></div><div class="c-tips-container" id="c-tips-container"></div>
<script>window.__async_strategy=2;</script>
<script>var bds={se:{},su:{urdata:[],urSendClick:function(){}},util:{},use:{},comm : {domain:"http://www.baidu.com",ubsurl : "http://sclick.baidu.com/w.gif",tn:"baidu",queryEnc:"",queryId:"",inter:"",templateName:"baidu",sugHost : "http://suggestion.baidu.com/su",query : "",qid : "",cid : "",sid : "",indexSid : "",stoken : "",serverTime : "",user : "",username : "",loginAction : [],useFavo : "",pinyin : "",favoOn : "",curResultNum:"",rightResultExist:false,protectNum:0,zxlNum:0,pageNum:1,pageSize:10,newindex:0,async:1,maxPreloadThread:5,maxPreloadTimes:10,preloadMouseMoveDistance:5,switchAddMask:false,isDebug:false,ishome : 1},_base64:{domain : "http://b1.bdstatic.com/",b64Exp : -1,pdc : 0}};var name,navigate,al_arr=[];var selfOpen = window.open;eval("var open = selfOpen;");var isIE=navigator.userAgent.indexOf("MSIE")!=-1&&!window.opera;var E = bds.ecom= {};bds.se.mon = {''loadedItems'':[],''load'':function(){},''srvt'':-1};try {bds.se.mon.srvt = parseInt(document.cookie.match(new RegExp("(^| )BDSVRTM=([^;]*)(;|$)"))[2]);document.cookie="BDSVRTM=;expires=Sat, 01 Jan 2000 00:00:00 GMT"; }catch(e){}</script>
<script>if(!location.hash.match(/[^a-zA-Z0-9]wd=/)){document.getElementById("ftCon").style.display=''block'';document.getElementById("u1").style.display=''block'';document.getElementById("content").style.display=''block'';document.getElementById("wrapper").style.display=''block'';setTimeout(function(){try{document.getElementById("kw1").focus();document.getElementById("kw1").parentNode.className += '' iptfocus'';}catch(e){}},0);}</script>
<script type="text/javascript" src="http://s1.bdstatic.com/r/www/cache/static/jquery/jquery-1.10.2.min_f2fb5194.js"></script>
<script>(function(){var index_content = $(''#content'');var index_foot= $(''#ftCon'');var index_css= $(''head [index]'');var index_u= $(''#u1'');var result_u= $(''#u'');var wrapper=$("#wrapper");window.index_on=function(){index_css.insertAfter("meta:eq(0)");result_common_css.remove();result_aladdin_css.remove();result_sug_css.remove();index_content.show();index_foot.show();index_u.show();result_u.hide();wrapper.show();if(bds.su&&bds.su.U&&bds.su.U.homeInit){bds.su.U.homeInit();}setTimeout(function(){try{$(''#kw1'').get(0).focus();window.sugIndex.start();}catch(e){}},0);if(typeof initIndex==''function''){initIndex();}};window.index_off=function(){index_css.remove();index_content.hide();index_foot.hide();index_u.hide();result_u.show();result_aladdin_css.insertAfter("meta:eq(0)");result_common_css.insertAfter("meta:eq(0)");result_sug_css.insertAfter("meta:eq(0)");wrapper.show();};})();</script>
<script>window.__switch_add_mask=1;</script>
<script type="text/javascript" src="http://s1.bdstatic.com/r/www/cache/static/global/js/instant_search_newi_redirect1_20bf4036.js"></script>
<script>initPreload();$("#u,#u1").delegate("#lb",''click'',function(){try{bds.se.login.open();}catch(e){}});if(navigator.cookieEnabled){document.cookie="NOJS=;expires=Sat, 01 Jan 2000 00:00:00 GMT";}</script>
<script>$(function(){for(i=0;i<3;i++){u($($(''.s_ipt_wr'')[i]),$($(''.s_ipt'')[i]),$($(''.s_btn_wr'')[i]),$($(''.s_btn'')[i]));}function u(iptwr,ipt,btnwr,btn){if(iptwr && ipt){iptwr.on(''mouseover'',function(){iptwr.addClass(''ipthover'');}).on(''mouseout'',function(){iptwr.removeClass(''ipthover'');}).on(''click'',function(){ipt.focus();});ipt.on(''focus'',function(){iptwr.addClass(''iptfocus'');}).on(''blur'',function(){iptwr.removeClass(''iptfocus'');}).on(''render'',function(e){var $s = iptwr.parent().find(''.bdsug'');var l = $s.find(''li'').length;if(l>=5){$s.addClass(''bdsugbg'');}else{$s.removeClass(''bdsugbg'');}});}if(btnwr && btn){btnwr.on(''mouseover'',function(){btn.addClass(''btnhover'');}).on(''mouseout'',function(){btn.removeClass(''btnhover'');});}}});</script>
<script type="text/javascript" src="http://s1.bdstatic.com/r/www/cache/static/home/js/bri_7f1fa703.js"></script>
<script>(function(){var _init=false;window.initIndex=function(){if(_init){return;}_init=true;var w=window,d=document,n=navigator,k=d.f1.wd,a=d.getElementById("nv").getElementsByTagName("a"),isIE=n.userAgent.indexOf("MSIE")!=-1&&!window.opera;(function(){if(/q=([^&]+)/.test(location.search)){k.value=decodeURIComponent(RegExp["\x241"])}})();(function(){var u = G("u1").getElementsByTagName("a"), nv = G("nv").getElementsByTagName("a"), lk = G("lk").getElementsByTagName("a"), un = "";var tj_nv = ["news","tieba","zhidao","mp3","img","video","map"];var tj_lk = ["baike","wenku","hao123","more"];un = bds.comm.user == "" ? "" : bds.comm.user;function _addTJ(obj){addEV(obj, "mousedown", function(e){var e = e || window.event;var target = e.target || e.srcElement;if(target.name){ns_c({''fm'':''behs'',''tab'':target.name,''un'':encodeURIComponent(un)});}});}for(var i = 0; i < u.length; i++){_addTJ(u[i]);}for(var i = 0; i < nv.length; i++){nv[i].name = ''tj_'' + tj_nv[i];}for(var i = 0; i < lk.length; i++){lk[i].name = ''tj_'' + tj_lk[i];}})();(function() {var links = {''tj_news'': [''word'', ''http://news.baidu.com/ns?tn=news&cl=2&rn=20&ct=1&ie=utf-8''],''tj_tieba'': [''kw'', ''http://tieba.baidu.com/f?ie=utf-8''],''tj_zhidao'': [''word'', ''http://zhidao.baidu.com/search?pn=0&rn=10&lm=0''],''tj_mp3'': [''key'', ''http://music.baidu.com/search?fr=ps&ie=utf-8''],''tj_img'': [''word'', ''http://image.baidu.com/i?ct=201326592&cl=2&nc=1&lm=-1&st=-1&tn=baiduimage&istype=2&fm=&pv=&z=0&ie=utf-8''],''tj_video'': [''word'', ''http://video.baidu.com/v?ct=301989888&s=25&ie=utf-8''],''tj_map'': [''wd'', ''http://map.baidu.com/?newmap=1&ie=utf-8&s=s''],''tj_baike'': [''word'', ''http://baike.baidu.com/search/word?pic=1&sug=1&enc=utf8''],''tj_wenku'': [''word'', ''http://wenku.baidu.com/search?ie=utf-8'']};var domArr = [G(''nv''), G(''lk''),G(''cp'')],kw = G(''kw1'');for (var i = 0, l = domArr.length; i < l; i++) {domArr[i].onmousedown = function(e) {e = e || window.event;var target = e.target || e.srcElement,name = target.getAttribute(''name''),items = links[name],reg = new RegExp(''^\\s+|\\s+\x24''),key = kw.value.replace(reg, '''');if (items) {if (key.length > 0) {var wd = items[0], url = items[1],url = url + ( name === ''tj_map'' ? encodeURIComponent(''&'' + wd + ''='' + key) : ( ( url.indexOf(''?'') > 0 ? ''&'' : ''?'' ) + wd + ''='' + encodeURIComponent(key) ) );target.href = url;} else {target.href = target.href.match(new RegExp(''^http:\/\/.+\.baidu\.com''))[0];}}name && ns_c({''fm'': ''behs'',''tab'': name,''query'': encodeURIComponent(key),''un'': encodeURIComponent(bds.comm.user || '''') });};}})();};if(window.pageState==0){initIndex();}})();document.cookie = ''IS_STATIC=1;expires='' + new Date(new Date().getTime() + 10*60*1000).toGMTString();</script>
</body></html>
', NULL);
INSERT INTO "t_job_record" VALUES ('52ea01d13f80c890b3ef08d5fedb8971', '46d7dca5abe5cf21cf8908d5f9611836', 1, '2018-08-10 16:08:40', '2018-08-10 16:08:40', 'code:OK,content:<!DOCTYPE html><!--STATUS OK-->
<html>
<head>
	<meta http-equiv="content-type" content="text/html;charset=utf-8">
	<meta http-equiv="X-UA-Compatible" content="IE=Edge">
	<link rel="dns-prefetch" href="//s1.bdstatic.com"/>
	<link rel="dns-prefetch" href="//t1.baidu.com"/>
	<link rel="dns-prefetch" href="//t2.baidu.com"/>
	<link rel="dns-prefetch" href="//t3.baidu.com"/>
	<link rel="dns-prefetch" href="//t10.baidu.com"/>
	<link rel="dns-prefetch" href="//t11.baidu.com"/>
	<link rel="dns-prefetch" href="//t12.baidu.com"/>
	<link rel="dns-prefetch" href="//b1.bdstatic.com"/>
	<title>百度一下，你就知道</title>
	<link href="http://s1.bdstatic.com/r/www/cache/static/home/css/index.css" rel="stylesheet" type="text/css" />
	<!--[if lte IE 8]><style index="index" >#content{height:480px\9}#m{top:260px\9}</style><![endif]-->
	<!--[if IE 8]><style index="index" >#u1 a.mnav,#u1 a.mnav:visited{font-family:simsun}</style><![endif]-->
	<script>var hashMatch = document.location.href.match(/#+(.*wd=[^&].+)/);if (hashMatch && hashMatch[0] && hashMatch[1]) {document.location.replace("http://"+location.host+"/s?"+hashMatch[1]);}var ns_c = function(){};</script>
	<script>function h(obj){obj.style.behavior=''url(#default#homepage)'';var a = obj.setHomePage(''//www.baidu.com/'');}</script>
	<noscript><meta http-equiv="refresh" content="0; url=/baidu.html?from=noscript"/></noscript>
	<script>window._ASYNC_START=new Date().getTime();</script>
</head>
<body link="#0000cc"><div id="wrapper" style="display:none;"><div id="u"><a href="//www.baidu.com/gaoji/preferences.html"  onmousedown="return user_c({''fm'':''set'',''tab'':''setting'',''login'':''0''})">搜索设置</a>|<a id="btop" href="/"  onmousedown="return user_c({''fm'':''set'',''tab'':''index'',''login'':''0''})">百度首页</a>|<a id="lb" href="https://passport.baidu.com/v2/?login&tpl=mn&u=http%3A%2F%2Fwww.baidu.com%2F" onclick="return false;"  onmousedown="return user_c({''fm'':''set'',''tab'':''login''})">登录</a><a href="https://passport.baidu.com/v2/?reg&regType=1&tpl=mn&u=http%3A%2F%2Fwww.baidu.com%2F"  onmousedown="return user_c({''fm'':''set'',''tab'':''reg''})" target="_blank" class="reg">注册</a></div><div id="head"><div class="s_nav"><a href="/" class="s_logo" onmousedown="return c({''fm'':''tab'',''tab'':''logo''})"><img src="//www.baidu.com/img/baidu_jgylogo3.gif" width="117" height="38" border="0" alt="到百度首页" title="到百度首页"></a><div class="s_tab" id="s_tab"><a href="http://news.baidu.com/ns?cl=2&rn=20&tn=news&word=" wdfield="word"  onmousedown="return c({''fm'':''tab'',''tab'':''news''})">新闻</a>&#12288;<b>网页</b>&#12288;<a href="http://tieba.baidu.com/f?kw=&fr=wwwt" wdfield="kw"  onmousedown="return c({''fm'':''tab'',''tab'':''tieba''})">贴吧</a>&#12288;<a href="http://zhidao.baidu.com/q?ct=17&pn=0&tn=ikaslist&rn=10&word=&fr=wwwt" wdfield="word"  onmousedown="return c({''fm'':''tab'',''tab'':''zhidao''})">知道</a>&#12288;<a href="http://music.baidu.com/search?fr=ps&key=" wdfield="key"  onmousedown="return c({''fm'':''tab'',''tab'':''music''})">音乐</a>&#12288;<a href="http://image.baidu.com/i?tn=baiduimage&ps=1&ct=201326592&lm=-1&cl=2&nc=1&word=" wdfield="word"  onmousedown="return c({''fm'':''tab'',''tab'':''pic''})">图片</a>&#12288;<a href="http://v.baidu.com/v?ct=301989888&rn=20&pn=0&db=0&s=25&word=" wdfield="word"   onmousedown="return c({''fm'':''tab'',''tab'':''video''})">视频</a>&#12288;<a href="http://map.baidu.com/m?word=&fr=ps01000" wdfield="word"  onmousedown="return c({''fm'':''tab'',''tab'':''map''})">地图</a>&#12288;<a href="http://wenku.baidu.com/search?word=&lm=0&od=0" wdfield="word"  onmousedown="return c({''fm'':''tab'',''tab'':''wenku''})">文库</a>&#12288;<a href="//www.baidu.com/more/"  onmousedown="return c({''fm'':''tab'',''tab'':''more''})">更多»</a></div></div><form id="form" name="f" action="/s" class="fm" ><input type="hidden" name="ie" value="utf-8"><input type="hidden" name="f" value="8"><input type="hidden" name="rsv_bp" value="1"><span class="bg s_ipt_wr"><input name="wd" id="kw" class="s_ipt" value="" maxlength="100"></span><span class="bg s_btn_wr"><input type="submit" id="su" value="百度一下" class="bg s_btn" onmousedown="this.className=''bg s_btn s_btn_h''" onmouseout="this.className=''bg s_btn''"></span><span class="tools"><span id="mHolder"><div id="mCon"><span>输入法</span></div><ul id="mMenu"><li><a href="javascript:;" name="ime_hw">手写</a></li><li><a href="javascript:;" name="ime_py">拼音</a></li><li class="ln"></li><li><a href="javascript:;" name="ime_cl">关闭</a></li></ul></span><span class="shouji"><strong>推荐&nbsp;:&nbsp;</strong><a href="http://w.x.baidu.com/go/mini/8/10000020" onmousedown="return ns_c({''fm'':''behs'',''tab'':''bdbrowser''})">百度浏览器，打开网页快2秒！</a></span></span></form></div><div id="content"><div id="u1"><a href="http://news.baidu.com" name="tj_trnews" class="mnav">新闻</a><a href="http://www.hao123.com" name="tj_trhao123" class="mnav">hao123</a><a href="http://map.baidu.com" name="tj_trmap" class="mnav">地图</a><a href="http://v.baidu.com" name="tj_trvideo" class="mnav">视频</a><a href="http://tieba.baidu.com" name="tj_trtieba" class="mnav">贴吧</a><a href="https://passport.baidu.com/v2/?login&tpl=mn&u=http%3A%2F%2Fwww.baidu.com%2F" name="tj_login" id="lb" onclick="return false;">登录</a><a href="//www.baidu.com/gaoji/preferences.html" name="tj_settingicon" id="pf">设置</a><a href="//www.baidu.com/more/" name="tj_briicon" id="bri">更多产品</a></div><div id="m"><p id="lg"><img src="//www.baidu.com/img/bd_logo.png" width="270" height="129"></p><p id="nv"><a href="http://news.baidu.com">新&nbsp;闻</a>　<b>网&nbsp;页</b>　<a href="http://tieba.baidu.com">贴&nbsp;吧</a>　<a href="http://zhidao.baidu.com">知&nbsp;道</a>　<a href="http://music.baidu.com">音&nbsp;乐</a>　<a href="http://image.baidu.com">图&nbsp;片</a>　<a href="http://v.baidu.com">视&nbsp;频</a>　<a href="http://map.baidu.com">地&nbsp;图</a></p><div id="fm"><form id="form1" name="f1" action="/s" class="fm"><span class="bg s_ipt_wr"><input type="text" name="wd" id="kw1" maxlength="100" class="s_ipt"></span><input type="hidden" name="rsv_bp" value="0"><input type=hidden name=ch value=""><input type=hidden name=tn value="baidu"><input type=hidden name=bar value=""><input type="hidden" name="rsv_spt" value="3"><input type="hidden" name="ie" value="utf-8"><span class="bg s_btn_wr"><input type="submit" value="百度一下" id="su1" class="bg s_btn" onmousedown="this.className=''bg s_btn s_btn_h''" onmouseout="this.className=''bg s_btn''"></span></form><span class="tools"><span id="mHolder1"><div id="mCon1"><span>输入法</span></div></span></span><ul id="mMenu1"><div class="mMenu1-tip-arrow"><em></em><ins></ins></div><li><a href="javascript:;" name="ime_hw">手写</a></li><li><a href="javascript:;" name="ime_py">拼音</a></li><li class="ln"></li><li><a href="javascript:;" name="ime_cl">关闭</a></li></ul></div><p id="lk"><a href="http://baike.baidu.com">百科</a>　<a href="http://wenku.baidu.com">文库</a>　<a href="http://www.hao123.com">hao123</a><span>&nbsp;|&nbsp;<a href="//www.baidu.com/more/">更多&gt;&gt;</a></span></p><p id="lm"></p></div></div><div id="ftCon"><div id="ftConw"><p id="lh"><a id="seth" onClick="h(this)" href="/" onmousedown="return ns_c({''fm'':''behs'',''tab'':''homepage'',''pos'':0})">把百度设为主页</a><a id="setf" href="//www.baidu.com/cache/sethelp/index.html" onmousedown="return ns_c({''fm'':''behs'',''tab'':''favorites'',''pos'':0})" target="_blank">把百度设为主页</a><a onmousedown="return ns_c({''fm'':''behs'',''tab'':''tj_about''})" href="http://home.baidu.com">关于百度</a><a onmousedown="return ns_c({''fm'':''behs'',''tab'':''tj_about_en''})" href="http://ir.baidu.com">About Baidu</a></p><p id="cp">&copy;2018&nbsp;Baidu&nbsp;<a href="/duty/" name="tj_duty">使用百度前必读</a>&nbsp;京ICP证030173号&nbsp;<img src="http://s1.bdstatic.com/r/www/cache/static/global/img/gs_237f015b.gif"></p></div></div><div id="wrapper_wrapper"></div></div><div class="c-tips-container" id="c-tips-container"></div>
<script>window.__async_strategy=2;</script>
<script>var bds={se:{},su:{urdata:[],urSendClick:function(){}},util:{},use:{},comm : {domain:"http://www.baidu.com",ubsurl : "http://sclick.baidu.com/w.gif",tn:"baidu",queryEnc:"",queryId:"",inter:"",templateName:"baidu",sugHost : "http://suggestion.baidu.com/su",query : "",qid : "",cid : "",sid : "",indexSid : "",stoken : "",serverTime : "",user : "",username : "",loginAction : [],useFavo : "",pinyin : "",favoOn : "",curResultNum:"",rightResultExist:false,protectNum:0,zxlNum:0,pageNum:1,pageSize:10,newindex:0,async:1,maxPreloadThread:5,maxPreloadTimes:10,preloadMouseMoveDistance:5,switchAddMask:false,isDebug:false,ishome : 1},_base64:{domain : "http://b1.bdstatic.com/",b64Exp : -1,pdc : 0}};var name,navigate,al_arr=[];var selfOpen = window.open;eval("var open = selfOpen;");var isIE=navigator.userAgent.indexOf("MSIE")!=-1&&!window.opera;var E = bds.ecom= {};bds.se.mon = {''loadedItems'':[],''load'':function(){},''srvt'':-1};try {bds.se.mon.srvt = parseInt(document.cookie.match(new RegExp("(^| )BDSVRTM=([^;]*)(;|$)"))[2]);document.cookie="BDSVRTM=;expires=Sat, 01 Jan 2000 00:00:00 GMT"; }catch(e){}</script>
<script>if(!location.hash.match(/[^a-zA-Z0-9]wd=/)){document.getElementById("ftCon").style.display=''block'';document.getElementById("u1").style.display=''block'';document.getElementById("content").style.display=''block'';document.getElementById("wrapper").style.display=''block'';setTimeout(function(){try{document.getElementById("kw1").focus();document.getElementById("kw1").parentNode.className += '' iptfocus'';}catch(e){}},0);}</script>
<script type="text/javascript" src="http://s1.bdstatic.com/r/www/cache/static/jquery/jquery-1.10.2.min_f2fb5194.js"></script>
<script>(function(){var index_content = $(''#content'');var index_foot= $(''#ftCon'');var index_css= $(''head [index]'');var index_u= $(''#u1'');var result_u= $(''#u'');var wrapper=$("#wrapper");window.index_on=function(){index_css.insertAfter("meta:eq(0)");result_common_css.remove();result_aladdin_css.remove();result_sug_css.remove();index_content.show();index_foot.show();index_u.show();result_u.hide();wrapper.show();if(bds.su&&bds.su.U&&bds.su.U.homeInit){bds.su.U.homeInit();}setTimeout(function(){try{$(''#kw1'').get(0).focus();window.sugIndex.start();}catch(e){}},0);if(typeof initIndex==''function''){initIndex();}};window.index_off=function(){index_css.remove();index_content.hide();index_foot.hide();index_u.hide();result_u.show();result_aladdin_css.insertAfter("meta:eq(0)");result_common_css.insertAfter("meta:eq(0)");result_sug_css.insertAfter("meta:eq(0)");wrapper.show();};})();</script>
<script>window.__switch_add_mask=1;</script>
<script type="text/javascript" src="http://s1.bdstatic.com/r/www/cache/static/global/js/instant_search_newi_redirect1_20bf4036.js"></script>
<script>initPreload();$("#u,#u1").delegate("#lb",''click'',function(){try{bds.se.login.open();}catch(e){}});if(navigator.cookieEnabled){document.cookie="NOJS=;expires=Sat, 01 Jan 2000 00:00:00 GMT";}</script>
<script>$(function(){for(i=0;i<3;i++){u($($(''.s_ipt_wr'')[i]),$($(''.s_ipt'')[i]),$($(''.s_btn_wr'')[i]),$($(''.s_btn'')[i]));}function u(iptwr,ipt,btnwr,btn){if(iptwr && ipt){iptwr.on(''mouseover'',function(){iptwr.addClass(''ipthover'');}).on(''mouseout'',function(){iptwr.removeClass(''ipthover'');}).on(''click'',function(){ipt.focus();});ipt.on(''focus'',function(){iptwr.addClass(''iptfocus'');}).on(''blur'',function(){iptwr.removeClass(''iptfocus'');}).on(''render'',function(e){var $s = iptwr.parent().find(''.bdsug'');var l = $s.find(''li'').length;if(l>=5){$s.addClass(''bdsugbg'');}else{$s.removeClass(''bdsugbg'');}});}if(btnwr && btn){btnwr.on(''mouseover'',function(){btn.addClass(''btnhover'');}).on(''mouseout'',function(){btn.removeClass(''btnhover'');});}}});</script>
<script type="text/javascript" src="http://s1.bdstatic.com/r/www/cache/static/home/js/bri_7f1fa703.js"></script>
<script>(function(){var _init=false;window.initIndex=function(){if(_init){return;}_init=true;var w=window,d=document,n=navigator,k=d.f1.wd,a=d.getElementById("nv").getElementsByTagName("a"),isIE=n.userAgent.indexOf("MSIE")!=-1&&!window.opera;(function(){if(/q=([^&]+)/.test(location.search)){k.value=decodeURIComponent(RegExp["\x241"])}})();(function(){var u = G("u1").getElementsByTagName("a"), nv = G("nv").getElementsByTagName("a"), lk = G("lk").getElementsByTagName("a"), un = "";var tj_nv = ["news","tieba","zhidao","mp3","img","video","map"];var tj_lk = ["baike","wenku","hao123","more"];un = bds.comm.user == "" ? "" : bds.comm.user;function _addTJ(obj){addEV(obj, "mousedown", function(e){var e = e || window.event;var target = e.target || e.srcElement;if(target.name){ns_c({''fm'':''behs'',''tab'':target.name,''un'':encodeURIComponent(un)});}});}for(var i = 0; i < u.length; i++){_addTJ(u[i]);}for(var i = 0; i < nv.length; i++){nv[i].name = ''tj_'' + tj_nv[i];}for(var i = 0; i < lk.length; i++){lk[i].name = ''tj_'' + tj_lk[i];}})();(function() {var links = {''tj_news'': [''word'', ''http://news.baidu.com/ns?tn=news&cl=2&rn=20&ct=1&ie=utf-8''],''tj_tieba'': [''kw'', ''http://tieba.baidu.com/f?ie=utf-8''],''tj_zhidao'': [''word'', ''http://zhidao.baidu.com/search?pn=0&rn=10&lm=0''],''tj_mp3'': [''key'', ''http://music.baidu.com/search?fr=ps&ie=utf-8''],''tj_img'': [''word'', ''http://image.baidu.com/i?ct=201326592&cl=2&nc=1&lm=-1&st=-1&tn=baiduimage&istype=2&fm=&pv=&z=0&ie=utf-8''],''tj_video'': [''word'', ''http://video.baidu.com/v?ct=301989888&s=25&ie=utf-8''],''tj_map'': [''wd'', ''http://map.baidu.com/?newmap=1&ie=utf-8&s=s''],''tj_baike'': [''word'', ''http://baike.baidu.com/search/word?pic=1&sug=1&enc=utf8''],''tj_wenku'': [''word'', ''http://wenku.baidu.com/search?ie=utf-8'']};var domArr = [G(''nv''), G(''lk''),G(''cp'')],kw = G(''kw1'');for (var i = 0, l = domArr.length; i < l; i++) {domArr[i].onmousedown = function(e) {e = e || window.event;var target = e.target || e.srcElement,name = target.getAttribute(''name''),items = links[name],reg = new RegExp(''^\\s+|\\s+\x24''),key = kw.value.replace(reg, '''');if (items) {if (key.length > 0) {var wd = items[0], url = items[1],url = url + ( name === ''tj_map'' ? encodeURIComponent(''&'' + wd + ''='' + key) : ( ( url.indexOf(''?'') > 0 ? ''&'' : ''?'' ) + wd + ''='' + encodeURIComponent(key) ) );target.href = url;} else {target.href = target.href.match(new RegExp(''^http:\/\/.+\.baidu\.com''))[0];}}name && ns_c({''fm'': ''behs'',''tab'': name,''query'': encodeURIComponent(key),''un'': encodeURIComponent(bds.comm.user || '''') });};}})();};if(window.pageState==0){initIndex();}})();document.cookie = ''IS_STATIC=1;expires='' + new Date(new Date().getTime() + 10*60*1000).toGMTString();</script>
</body></html>
', NULL);
COMMIT;

-- ----------------------------
-- Table structure for t_job_trigger
-- ----------------------------
DROP TABLE IF EXISTS "t_job_trigger";
CREATE TABLE "t_job_trigger" (
  "Id" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "JobId" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "Type" int4 NOT NULL,
  "Corn" varchar(128) COLLATE "pg_catalog"."default",
  "Times" int4 DEFAULT 0,
  "Interval" int4,
  "Start" timestamp(0),
  "Expired" timestamp(0),
  "PrevTime" timestamp(0),
  "NextTime" timestamp(0)
)
;
COMMENT ON COLUMN "t_job_trigger"."JobId" IS '任务ID';
COMMENT ON COLUMN "t_job_trigger"."Type" IS '触发器类型';
COMMENT ON COLUMN "t_job_trigger"."Corn" IS 'Corn表达式';
COMMENT ON COLUMN "t_job_trigger"."Times" IS '执行次数';
COMMENT ON COLUMN "t_job_trigger"."Interval" IS '执行间隔(秒)';
COMMENT ON COLUMN "t_job_trigger"."Start" IS '开始时间';
COMMENT ON COLUMN "t_job_trigger"."Expired" IS '结束时间';
COMMENT ON COLUMN "t_job_trigger"."PrevTime" IS '上次执行时间';
COMMENT ON COLUMN "t_job_trigger"."NextTime" IS '下次执行时间';

-- ----------------------------
-- Records of t_job_trigger
-- ----------------------------
BEGIN;
INSERT INTO "t_job_trigger" VALUES ('6875a3c96b14ce98339708d5f961184d', '46d7dca5abe5cf21cf8908d5f9611836', 1, '0/20 * * * * ?', 0, 30, NULL, NULL, '2018-08-10 16:08:40', NULL);
COMMIT;

-- ----------------------------
-- Primary Key structure for table t_job
-- ----------------------------
ALTER TABLE "t_job" ADD CONSTRAINT "t_job_pkey" PRIMARY KEY ("Id");

-- ----------------------------
-- Primary Key structure for table t_job_http
-- ----------------------------
ALTER TABLE "t_job_http" ADD CONSTRAINT "t_job_http_pkey" PRIMARY KEY ("Id");

-- ----------------------------
-- Primary Key structure for table t_job_record
-- ----------------------------
ALTER TABLE "t_job_record" ADD CONSTRAINT "t_job_record_pkey" PRIMARY KEY ("Id");

-- ----------------------------
-- Primary Key structure for table t_job_trigger
-- ----------------------------
ALTER TABLE "t_job_trigger" ADD CONSTRAINT "t_job_trigger_pkey" PRIMARY KEY ("Id");
