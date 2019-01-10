<template>
  <el-dialog
    :visible.sync="dialogVisible"
    :title="value.name + ' - 任务日志'"
    width="60%">
    <el-table
      :data="list">
      <el-table-column type="expand">
        <template slot-scope="scope">
          {{ scope.row.result }}
        </template>
      </el-table-column>
      <el-table-column :index="indexMethod" type="index"/>
      <el-table-column label="开始时间" prop="startTime">
        <template slot-scope="scope">
          {{ scope.row.startTime | parseTime() }}
        </template>
      </el-table-column>
      <el-table-column label="结束时间" prop="completeTime">
        <template slot-scope="scope">
          {{ scope.row.completeTime | parseTime() }}
        </template>
      </el-table-column>
      <el-table-column label="耗时(ms)" prop="time">
        <template slot-scope="scope">
          {{ `${scope.row.time}ms` }}
        </template>
      </el-table-column>
      <el-table-column label="备注" prop="remark" />
      <el-table-column label="返回码" prop="resultCode" />
      <el-table-column label="状态" prop="status">
        <template slot-scope="scope">
          <el-tag :type="['','success','danger'][scope.row.status]">{{ scope.row.statusCn }}</el-tag>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      :total="total"
      :page-size.sync="size"
      :current-page.sync="page"
      layout="prev, pager, next"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"/>
    <span slot="footer" class="dialog-footer">
      <el-button type="primary" @click="dialogVisible = false">确 定</el-button>
    </span>
  </el-dialog>
</template>
<script>
import { logs } from '@/api/jobs'
export default {
  name: 'JobLogs',
  props: {
    value: {
      type: Object,
      required: true,
      default: () => {}
    },
    triggerId: {
      type: String,
      required: false,
      default: () => null
    },
    show: {
      type: Boolean,
      required: false,
      default: () => false
    }
  },
  data() {
    return {
      page: 1,
      size: 10,
      total: 0,
      dialogVisible: this.show,
      list: []
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
      if (!val || !val.id || !this.show) return
      this.page = 1
      this.getList()
    },
    triggerId() {
      this.page = 1
      this.getList()
    }
  },
  methods: {
    indexMethod(index) {
      return (this.page - 1) * this.size + index + 1
    },
    getList() {
      logs(this.value.id, this.triggerId, this.page, this.size).then(json => {
        this.total = json.total
        this.list = json.data
      })
    },
    handleSizeChange(size) {
      this.page = 1
      this.size = size
      this.getList()
    },
    handlePageChange(page) {
      this.page = page
      this.getList()
    }
  }

}
</script>

