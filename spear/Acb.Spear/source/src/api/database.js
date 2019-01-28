import request from '@/utils/request'

/**
 * 列表
 * @param {Object} search
 */
export const list = ({ type, page = 1, size = 10 }) => {
  return request.get('/api/database', {
    params: {
      type,
      page,
      size
    }
  })
}

/**
 * 添加
 * @param {*} param0
 */
export const add = ({ name, type, connectionString }) => {
  return request.post('/api/database', { name, type, connectionString })
}

/**
 * 修改
 * @param {*} id
 * @param {*} name
 * @param {*} type
 * @param {*} connectionString
 */
export const edit = (id, name, type, connectionString) => {
  return request.put(`/api/database/${id}`, {
    name, type, connectionString
  })
}

/**
 * 删除
 * @param {*} id
 */
export const remove = id => {
  return request.delete(`/api/database/${id}`)
}

export const viewUrl = id => {
  return `${process.env.BASE_API}/tables/${id}`
}
