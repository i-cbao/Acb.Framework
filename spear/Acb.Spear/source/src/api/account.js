import request from '@/utils/request'

/**
 * 帐号登录
 * @param {*} account
 * @param {*} password
 */
export const login = (account, password) => {
  return request.post('/api/account/login', {
    account,
    password
  })
}

/**
 * 获取帐号信息
 */
export const getInfo = () => {
  return request.get('/api/account')
}
