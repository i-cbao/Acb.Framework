<template>
  <div class="app-container">
    <div class="filter-container">
      <el-input
        v-model="query.keyword"
        placeholder="输入关键字"
        style="width: 200px;"
        class="filter-item"
      />
      <el-select
        v-model="query.status"
        :placeholder="$t('table.status')"
        clearable
        class="filter-item"
        style="width: 130px"
      >
        <el-option
          v-for="item in statusList"
          :key="item.key"
          :label="item.value"
          :value="item.key"
        />
      </el-select>
      <el-button
        class="filter-item"
        type="primary"
        icon="el-icon-search"
        @click="handleSearch"
      >{{ $t('table.search') }}</el-button>
      <el-button
        class="filter-item"
        type="default"
        icon="el-icon-plus"
        @click="handleCreate()"
      >{{ $t('jobTable.create') }}</el-button>
    </div>
    <el-table
      v-loading="loading"
      :data="list"
    >
      <el-table-column type="expand">
        <template slot-scope="scope">
          <el-form
            inline
            class="table-expand"
            label-width="100px"
          >
            <el-form-item label="method">{{ ['Get','Post','Delete','Put','Patch'][scope.row.detail.method] }}</el-form-item>
            <el-form-item label="url">{{ scope.row.detail.url }}</el-form-item>
            <el-form-item label="data">{{ scope.row.detail.data }}</el-form-item>
          </el-form>
          <triggers :job="scope.row" @edit="handleTriggerEdit" @logs="handleTriggerLogs" />
        </template>
      </el-table-column>
      <el-table-column
        :index="indexMethod"
        type="index"
      />
      <el-table-column
        label="任务分组"
        prop="group"
      />
      <el-table-column
        label="任务名称"
        prop="name"
      />
      <el-table-column
        label="任务类型"
        prop="type"
      >
        <template slot-scope="scope">
          <el-tag :type="['','warning','success','danger'][scope.row.type]">{{ scope.row.typeCn }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column
        label="上次执行时间"
        prop="prevTime"
      >
        <template slot-scope="scope">
          {{ scope.row.prevTime | parseTime() }}
        </template>
      </el-table-column>
      <el-table-column
        label="下次执行时间"
        prop="nextTime"
      >
        <template slot-scope="scope">
          {{ scope.row.nextTime | parseTime() }}
        </template>
      </el-table-column>
      <el-table-column
        label="创建时间"
        prop="creationTime"
      >
        <template slot-scope="scope">
          {{ scope.row.creationTime | parseTime() }}
        </template>
      </el-table-column>
      <el-table-column label="任务状态" prop="status" width="100" >
        <template slot-scope="scope">
          <el-switch
            :value="scope.row.status"
            :title="scope.row.statusCn"
            :active-value="1"
            :inactive-value="0"
            :disabled="scope.row.status === 4"
            active-color="#13ce66"
            inactive-color="#ff4949"
            @change="handleStatusChange(scope.row)"/>
        </template>
      </el-table-column>
      <el-table-column
        :label="$t('table.actions')"
        align="center"
        width="220"
        class-name="small-padding fixed-width"
      >
        <template slot-scope="scope">
          <el-button
            v-if="scope.row.status !== 4"
            :title="$t('table.edit')"
            type="primary"
            size="mini"
            icon="el-icon-edit"
            circle
            @click="handleCreate(scope.row)"
          />
          <el-button
            :title="$t('jobTable.logs')"
            type="info"
            size="mini"
            icon="el-icon-more"
            circle
            @click="handleLogs(scope.row)"
          />
          <el-button
            v-if="scope.row.status === 4"
            :title="$t('jobTable.resume')"
            type="success"
            size="mini"
            icon="el-icon-caret-left"
            circle
            @click="handleUpdate(scope.row, 0)"
          />
          <el-button
            v-if="scope.row.status !== 4"
            :title="$t('jobTable.trigger')"
            size="mini"
            type="success"
            icon="el-icon-time"
            circle
            @click="handleTrigger(scope.row)"
          />
          <el-button
            v-if="scope.row.status !== 4"
            :title="$t('jobTable.action')"
            type="warning"
            size="mini"
            icon="el-icon-caret-right"
            circle
            @click="handleRun(scope.row)"/>
          <el-button
            v-if="scope.row.status !== 4"
            :title="$t('table.delete')"
            type="danger"
            size="mini"
            icon="el-icon-delete"
            circle
            @click="handleUpdate(scope.row, 4)"
          />
        </template>
      </el-table-column>
    </el-table>
    <logs :value="currentJob" :trigger-id="currentTriggerId" :show="showLogs" @visibleChange="handleLogsVisible" />
    <create :value="currentJob" :show="showCreate" @visibleChange="handleCreateVisible" @success="getList()" />
    <trigger :value="currentTrigger" :job="currentJob" :show="showTrigger" @visibleChange="handleTriggerVisible" />
  </div>
</template>
<script>
import * as jobApi from '@/api/jobs'
import Logs from './dialogs/Logs'
import Create from './dialogs/Create'
import Trigger from './dialogs/Trigger'
import Triggers from './components/Triggers'
export default {
  name: 'Jobs',
  components: {
    Create,
    Logs,
    Trigger,
    Triggers
  },
  data() {
    return {
      query: {
        keyword: '',
        status: null,
        page: 1,
        size: 10
      },
      statusList: [
        {
          key: 0,
          value: '已暂停'
        },
        {
          key: 1,
          value: '已启动'
        },
        {
          key: 2,
          value: '异常'
        },
        {
          key: 4,
          value: '已删除'
        }
      ],
      currentJob: {
        detail: {}
      },
      currentTrigger: {},
      currentTriggerId: null,
      showLogs: false,
      showCreate: false,
      showTrigger: false,
      loading: false,
      total: 0,
      list: []
    }
  },
  mounted() {
    this.getList()
  },
  methods: {
    indexMethod(index) {
      return (this.query.page - 1) * this.query.size + index + 1
    },
    getList() {
      this.loading = true
      jobApi.list(this.query).then(json => {
        this.total = json.total
        this.list = json.data
        this.loading = false
      }).catch(e => {
        this.loading = false
      })
    },
    handleSearch() {
      this.query.page = 1
      this.getList()
    },
    handleUpdate(row, status) {
      var text = status === 1 ? '开启' : (status === 0 ? '暂停' : (status === 4 ? '删除' : ''))
      if (row.status === 4 && status === 0) {
        text = '恢复'
      }
      this.$confirm(`确认要${text}任务吗？`).then(() => {
        if (status === 1) {
          jobApi.resume(row.id).then(() => {
            this.$message.success(`任务已${text}`)
            this.getList()
          })
        } else if (status === 0) {
          jobApi.pause(row.id).then(() => {
            this.$message.success(`任务已${text}`)
            this.getList()
          })
        } else if (status === 4) {
          jobApi.remove(row.id).then(() => {
            this.$message.success(`任务已${text}`)
            this.getList()
          })
        }
      })
    },
    handleStatusChange(row) {
      if (row.status === 0) { return this.handleUpdate(row, 1) }
      if (row.status === 1) { return this.handleUpdate(row, 0) }
    },
    handleRun(row) {
      this.$confirm('确认要立即执行任务？').then(() => {
        jobApi.runJob(row.id).then(() => {
          this.$message.success(`任务已执行成功`)
          this.getList()
        })
      })
    },
    handleCreate(job) {
      this.currentJob = Object.assign({}, job || {})
      this.showCreate = true
    },
    handleLogs(job) {
      this.currentJob = Object.assign({}, job || {})
      this.showLogs = true
    },
    handleTrigger(job) {
      // 添加触发器
      this.currentJob = job
      this.showTrigger = true
    },
    handleLogsVisible(visible) {
      this.showLogs = visible
    },
    handleCreateVisible(visible) {
      this.showCreate = visible
    },
    handleTriggerVisible(visible) {
      this.showTrigger = visible
    },
    handleTriggerEdit(job, trigger) {
      // 添加触发器
      this.currentJob = job
      this.currentTrigger = trigger
      this.showTrigger = true
    },
    handleTriggerLogs(job, triggerId) {
      this.currentJob = job
      this.currentTriggerId = triggerId
      this.showLogs = true
    }
  }
}
</script>
