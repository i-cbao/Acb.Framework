import ajax from '@/utils/request'

import {
  setToken
} from '@/utils/auth'

const _url = (key, env) => {
  if (env === 'default') { return `api/config/${key}` }
  return `api/config/${key}/${env}`
}

export const list = () => ajax.get('api/config/list')

export const config = (key, env) => ajax.get(`api/config/${key}/${env}`)

export const save = (key, env, content) => ajax.post(_url(key, env), content)

export const remove = (key, env) => ajax.delete(_url(key, env))

export const login = (account, password) => {
  return ajax.post('api/config/login', {
    account,
    password
  }).then(json => {
    setToken(json)
  })
}
