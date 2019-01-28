<style>
.jsoneditor-poweredBy {
  display: none;
}
</style>

<style rel="stylesheet/scss" lang="scss" scoped>
.el-row {
  margin-bottom: 20px;
}
#jsoneditor {
  height: 500px;
  width: 100%;
}
</style>
<template>
  <el-container style="padding:20px">
    <!-- <el-aside width="220px">
            <el-menu @select="selectKey">
                <el-menu-item v-for="item in list" :key="item" :index="item">{{item}}</el-menu-item>
            </el-menu>
        </el-aside> -->
    <el-main>
      <el-row :gutter="20">
        <el-col :span="4">
          <!-- <el-input type="text" placeholder="请输入配置项" v-model="configKey"></el-input> -->
          <el-select v-model="configKey" filterable allow-create placeholder="请输入配置项" @change="envChange">
            <el-option v-for="item in list" :key="item" :label="item" :value="item" />
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-select v-model="configEnv" placeholder="请选择环境" @change="envChange">
            <el-option v-for="env in envs" :key="env" :label="env" :value="env" />
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-select v-model="configMode" placeholder="请选择编辑模式" @change="modeChange">
            <el-option v-for="mode in modes" :key="mode" :label="mode" :value="mode" />
          </el-select>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="16">
          <div id="jsoneditor" />
        </el-col>
      </el-row>
      <el-row>
        <el-button :disabled="!saveStatus" type="primary" @click="save">保存配置</el-button>
        <el-button :disabled="!saveStatus" type="danger" @click="remove">删除配置</el-button>
      </el-row>
    </el-main>
  </el-container>
</template>
<script>
import { list, config, save, remove } from '@/api/config'
import JSONEditor from 'jsoneditor'
import 'jsoneditor/dist/jsoneditor.min.css'
export default {
  name: 'Config',
  data() {
    return {
      envs: ['default', 'dev', 'test', 'ready', 'prod'],
      modes: ['text', 'code', 'tree', 'view', 'form'],
      configMode: 'view',
      configEnv: 'default',
      list: [],
      configKey: '',
      config: '',
      editor: false
    }
  },
  computed: {
    saveStatus() {
      return this.configKey && this.configKey.length > 1
    },
    deleteStatus() {
      return this.configKey && this.configKey.length > 1
    }
  },
  created() {
    this.loadList()
  },
  mounted() {
    this.editor = new JSONEditor(document.getElementById('jsoneditor'), {
      mode: this.configMode,
      search: true,
      height: 500
    })
  },
  methods: {
    loadList() {
      list().then(json => {
        this.list = json.data
      })
    },
    loadConfig() {
      if (!this.configKey) return
      config(this.configKey, this.configEnv).then(json => {
        this.editor.set(json || {})
        if (
          this.configMode === 'tree' ||
          this.configMode === 'view' ||
          this.configMode === 'form'
        ) {
          this.editor.expandAll()
        }
      })
    },
    selectKey(key) {
      this.configKey = key
      this.loadConfig()
    },
    envChange(env) {
      this.loadConfig()
    },
    modeChange() {
      this.editor.setMode(this.configMode)
    },
    save() {
      this.config = this.editor.getText() || '{}'
      var confObj
      try {
        confObj = JSON.parse(this.config)
      } catch (e) {
        console.log(e)
      }
      if (typeof confObj !== 'object') {
        this.$message({
          message: '配置信息必须为Json格式',
          type: 'warning'
        })
        return false
      }
      this.isLoading = true
      save(this.configKey, this.configEnv, confObj).then(json => {
        this.$message({
          message: '配置文件保存成功',
          type: 'success'
        })
        this.loadList()
        this.isLoading = false
      })
    },
    remove() {
      this.$confirm('此操作将删除该配置文件, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        remove(this.configKey, this.configEnv).then(json => {
          this.$message({
            message: '配置文件删除成功',
            type: 'success'
          })
          this.loadList()
          this.loadConfig()
        })
      })
    }
  }
}
</script>

