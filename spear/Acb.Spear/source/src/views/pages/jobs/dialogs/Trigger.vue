<template>
  <el-dialog
    :visible.sync="dialogVisible"
    :title="title"
    width="500px">
    <el-form label-width="100px">
      <el-form-item label="类型">
        <el-select v-model="model.type" placeholder="选择类型">
          <el-option :key="1" :value="1" label="Corn"/>
          <el-option :key="2" :value="2" label="Simple"/>
        </el-select>
      </el-form-item>
      <el-form-item label="开始时间">
        <el-date-picker
          v-model="model.start"
          type="datetime"
          placeholder="选择日期时间"/>
      </el-form-item>
      <el-form-item label="结束时间">
        <el-date-picker
          v-model="model.expired"
          type="datetime"
          placeholder="选择结束时间"/>
      </el-form-item>
      <el-form-item v-if="model.type === 1" label="Corn表达式">
        <el-input v-model="model.corn" placeholder="请输入Corn表达式" style="width:20rem;" />
      </el-form-item>
      <template v-if="model.type === 2">
        <el-form-item label="间隔">
          <el-input-number v-model="model.interval" :min="1" placeholder="请输入请求参数" style="width:10rem;" />
        </el-form-item>
        <el-form-item label="重复次数">
          <el-input-number v-model="model.times" :min="-1" placeholder="请输入任务描述" style="width:10rem;" />
        </el-form-item>
      </template>
    </el-form>
    <span slot="footer" class="dialog-footer">
      <el-button @click="dialogVisible = false">取 消</el-button>
      <el-button type="primary" @click="handleSave">确 定</el-button>
    </span>
  </el-dialog>
</template>
<script>
import { addTrigger, updateTrigger } from '@/api/jobs'
export default {
  name: 'TriggerCreate',
  props: {
    value: {
      type: Object,
      required: false,
      default: () => {}
    },
    job: {
      type: Object,
      required: true
    },
    show: {
      type: Boolean,
      required: true,
      default: () => false
    }
  },
  data() {
    return {
      title: '创建触发器',
      dialogVisible: this.show,
      create: true,
      model: Object.assign({}, this.value)
    }
  },
  watch: {
    show(val) {
      this.dialogVisible = val
    },
    dialogVisible(val) {
      this.$emit('visibleChange', val)
    },
    value(val) {
      this.create = !val || !val.id
      if (this.create) {
        this.model = {}
        this.title = '创建触发器'
      } else {
        this.model = val
        this.title = '编辑触发器'
      }
    }
  },
  methods: {
    handleSave() {
      if (this.create) {
        addTrigger(this.job.id, this.model).then(json => {
          this.$message.success(`${this.title}成功`)
          this.dialogVisible = false
          this.$emit('success')
        })
      } else {
        updateTrigger(this.model.id, this.model).then(json => {
          this.$message.success(`${this.title}成功`)
          this.dialogVisible = false
          this.$emit('success')
        })
      }
    }
  }
}
</script>
