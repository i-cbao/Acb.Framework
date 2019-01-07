import request from '@/utils/request'

import {
  setToken
} from '@/utils/auth'

const _url = (key, env) => {
  if (env === 'default') { return `api/config/${key}` }
  return `api/config/${key}/${env}`
}

export const list = () => request.get('api/config/list')

export const config = (key, env) => request.get(`api/config/${key}/${env}`)

export const save = (key, env, content) => request.post(_url(key, env), content)

export const remove = (key, env) => request.delete(_url(key, env))

export const login = (account, password) => {
  return request.post('api/config/login', {
    account,
    password
  }).then(json => {
    setToken(json)
  })
}
