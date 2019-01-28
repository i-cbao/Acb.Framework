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
        placeholder="数据库类型"
        clearable
        class="filter-item"
        style="width: 130px"
      >
        <el-option
          v-for="item in providers"
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
      <el-table-column
        :index="indexMethod"
        type="index"
      />
      <el-table-column
        label="类型"
        prop="provider"
        width="150px"
      >
        <template slot-scope="scope">
          <el-tag :type="['','warning','success','danger'][scope.row.provider]">{{ scope.row.providerCn }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column
        label="名称"
        prop="name"
        width="200px"
      />
      <el-table-column
        label="连接字符"
        prop="connectionString"
      />
      <el-table-column
        label="创建时间"
        prop="createTime"
        width="190px"
      >
        <template slot-scope="scope">
          {{ scope.row.createTime | parseTime() }}
        </template>
      </el-table-column>
      <el-table-column
        :label="$t('table.actions')"
        align="center"
        width="220"
        class-name="small-padding fixed-width"
      >
        <template slot-scope="scope">
          <a
            :href="viewUrl(scope.row.id)"
            target="_blank"
            title="查看文档"
            class="el-button el-button--success el-button--mini is-circle"
          >
            <i class="el-icon-view" />
          </a>
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
            v-if="scope.row.status !== 4"
            :title="$t('table.delete')"
            type="danger"
            size="mini"
            icon="el-icon-delete"
            circle
          />
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
<script>
import { list, viewUrl } from '@/api/database'
export default {
  name: 'DbDoc',
  data() {
    return {
      query: {
        type: undefined,
        page: 1,
        size: 10
      },
      loading: false,
      total: 0,
      providers: [
        {
          key: 0,
          value: 'SqlServer'
        },
        {
          key: 1,
          value: 'MySql'
        },
        {
          key: 2,
          value: 'PostgreSql'
        },
        {
          key: 3,
          value: 'SQLite'
        }
      ],
      list: []
    }
  },
  mounted() {
    this.loadList()
  },
  methods: {
    indexMethod(index) {
      return (this.query.page - 1) * this.query.size + index + 1
    },
    loadList() {
      this.loading = true
      list(this.query)
        .then(json => {
          this.total = json.total
          this.list = json.data
          this.loading = false
        })
        .catch(e => {
          this.loading = false
        })
    },
    handleSearch() {
      this.page = 1
      this.loadList()
    },
    viewUrl(id) {
      return viewUrl(id)
    }
  }
}
</script>

