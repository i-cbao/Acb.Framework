<template>
  <div class="app-container">
    <!-- <div class="filter-container">
      <el-button size="mini" type="primary" icon="el-icon-plus">创建触发器</el-button>
    </div> -->
    <el-table
      :data="list">
      <!-- <el-table-column type="index"/> -->
      <el-table-column label="类型" prop="type">
        <template slot-scope="scope">
          <el-tag :type="['','success','info'][scope.row.type]">{{ scope.row.typeCn }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="开始时间" prop="start">
        <template slot-scope="scope">
          {{ scope.row.start | parseTime() }}
        </template>
      </el-table-column>
      <el-table-column label="结束时间" prop="expired">
        <template slot-scope="scope">
          {{ scope.row.expired | parseTime() }}
        </template>
      </el-table-column>
      <el-table-column label="Corn表达式" prop="corn">
        <template slot-scope="scope">
          {{ scope.row.type === 2 ? '--' : scope.row.corn }}
        </template>
      </el-table-column>
      <el-table-column label="间隔(秒)" prop="interval">
        <template slot-scope="scope">
          {{ scope.row.type === 1 ? '--' : `${scope.row.interval}秒` }}
        </template>
      </el-table-column>
      <el-table-column label="重复次数" prop="times">
        <template slot-scope="scope">
          {{ scope.row.type === 1 ? '--' : (scope.row.times >= 0 ? `${scope.row.times}次` : '永久') }}
        </template>
      </el-table-column>
      <el-table-column label="上次执行" prop="prevTime">
        <template slot-scope="scope">
          {{ scope.row.prevTime | parseTime() }}
        </template>
      </el-table-column>
      <el-table-column label="下次执行" prop="nextTime">
        <template slot-scope="scope">
          {{ scope.row.nextTime | parseTime() }}
        </template>
      </el-table-column>
      <el-table-column label="状态" prop="status" width="100">
        <template slot-scope="scope">
          <el-switch
            :value="scope.row.status"
            :title="scope.row.statusCn"
            :active-value="1"
            :inactive-value="0"
            :disabled="job.status === 4"
            active-color="#13ce66"
            inactive-color="#ff4949"
            @change="handleStatusChange(scope.row)"/>
        </template>
      </el-table-column>
      <el-table-column
        :label="$t('table.actions')"
        align="center"
        width="130"
        class-name="small-padding fixed-width"
      >
        <template slot-scope="scope">
          <el-button v-if="job.status !== 4" :title="$t('table.edit')" type="primary" size="mini" icon="el-icon-edit" circle @click="handleEdit(scope.row)"/>
          <el-button :title="$t('jobTable.logs')" type="info" size="mini" icon="el-icon-more" circle @click="handleLogs(scope.row)"/>
          <el-button v-if="job.status !== 4" :title="$t('table.delete')" type="danger" size="mini" icon="el-icon-delete" circle/>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
<script>
import { triggers, addTrigger, updateTrigger, updateTriggerStatus, removeTrigger } from '@/api/jobs'
export default {
  name: 'Triggers',
  props: {
    job: {
      type: Object,
      required: true,
      default: () => {}
    }
  },
  data() {
    return {
      total: 0,
      list: []
    }
  },
  mounted() {
    this.getList()
  },
  methods: {
    getList() {
      triggers(this.job.id).then(json => {
        this.total = json.total
        this.list = json.data
      })
    },
    handleSave() {
      if (this.current.id) {
        updateTrigger(this.job.id, this.current).then(() => {

        })
      } else {
        addTrigger(this.job.id, this.current).then(() => {

        })
      }
    },
    handleRemove(id) {
      removeTrigger(id).then(() => {
        this.$message.succss('删除触发器成功')
        this.getList()
      })
    },
    handleStatusChange(row) {
      var status = row.status === 0 ? 1 : 0
      updateTriggerStatus(row.id, status).then(() => {
        this.$message.success('状态更新成功')
        row.status = status
      })
    },
    handleEdit(row) {
      this.$emit('edit', this.job, row)
    },
    handleLogs(row) {
      this.$emit('logs', this.job, row.id)
    }
  }
}
</script>

