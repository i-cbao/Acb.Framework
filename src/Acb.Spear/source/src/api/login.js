import request from '@/utils/request'

export const loginByUsername = (account, password) => {
  return request.post('api/config/login', {
    account,
    password
  })
}

export const logout = () => {
  return request({
    url: '/login/logout',
    method: 'post'
  })
}

export const getUserInfo = token => {
  token = 'admin'
  return request({
    url: '/user/info',
    method: 'get',
    params: {
      token
    }
  })
}
