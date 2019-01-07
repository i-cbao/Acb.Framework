import request from '@/utils/request'

export const login = (account, password) => {
  return request.post('/api/account/login', {
    account,
    password
  })
}

export const logout = () => {
  return request.post('/api/account/logout')
}

export const getInfo = () => {
  return request.get('/api/account')
}
