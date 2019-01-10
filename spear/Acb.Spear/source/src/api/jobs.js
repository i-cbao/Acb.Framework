import request from '@/utils/request'

/**
 * 任务列表
 * @param {*} keyword
 * @param {*} status
 * @param {*} page
 * @param {*} size
 */
export const list = ({ keyword, status, page = 1, size = 10 }) => {
  return request.get(`/api/job/list`, {
    params: {
      keyword,
      status,
      page,
      size
    }
  })
}

/**
 * 任务详情
 * @param {*} id
 */
export const detail = id => {
  return request.get(`/api/job/${id}`)
}

/**
 * 添加任务
 * @param {*} job
 */
export const create = (job) => {
  return request.post('/api/job', job)
}

/**
 * 保存任务
 * @param {*} job
 */
export const save = (job) => {
  return request.put(`/api/job/${job.id}`, job)
}

/**
 * 暂停任务
 * @param {*} id
 */
export const pause = id => {
  return request.put(`/api/job/pause/${id}`)
}

/**
 * 恢复任务
 * @param {*} id
 */
export const resume = id => {
  return request.put(`/api/job/resume/${id}`)
}

/**
 * 执行任务
 * @param {*} id
 */
export const runJob = id => {
  return request.post(`/api/job/start/${id}`)
}

/**
 * 删除任务
 * @param {*} id
 */
export const remove = id => {
  return request.delete(`/api/job/${id}`)
}

/**
 * 获取任务日志
 * @param {*} id
 * @param {*} page
 * @param {*} size
 */
export const logs = (id, triggerId = null, page = 1, size = 10) => {
  return request.get(`/api/job/logs/${id}`, {
    params: {
      triggerId,
      page,
      size
    }
  })
}

/**
 * 获取触发器列表
 * @param {guid} id
 */
export const triggers = id => {
  return request.get(`/api/job/triggers/${id}`)
}

/**
 * 添加触发器
 * @param {guid} id
 * @param {*} trigger
 */
export const addTrigger = (id, trigger) => {
  if (trigger.start) {
    trigger.start = +new Date(trigger.start)
  }
  if (trigger.expired) {
    trigger.expired = +new Date(trigger.expired)
  }
  return request.post(`/api/job/trigger/${id}`, trigger)
}

/**
 * 更新触发器
 * @param {guid} id
 * @param {*} trigger
 */
export const updateTrigger = (id, trigger) => {
  if (trigger.start) {
    trigger.start = +new Date(trigger.start)
  }
  if (trigger.expired) {
    trigger.expired = +new Date(trigger.expired)
  }
  return request.put(`/api/job/trigger/${id}`, trigger)
}

/**
 * 更新触发器状态
 * @param {*} id
 * @param {*} status
 */
export const updateTriggerStatus = (id, status) => {
  return request.put(`/api/job/trigger/${id}/status`, null, {
    params: {
      status
    }
  })
}

/**
 * 删除触发器
 * @param {guid} id
 * @param {*} trigger
 */
export const removeTrigger = triggerId => {
  return request.delete(`/api/job/trigger/${triggerId}`)
}
