import request from '@/utils/request'

const _url = (key, env) => {
  if (env === 'default') {
    return `/api/config/${key}`
  }
  return `/api/config/${key}/${env}`
}

export const list = () => request.get('/api/config/list')

export const config = (key, env) => request.get(`/config/${key}/${env}`)

export const save = (key, env, content) => request.post(_url(key, env), content)

export const remove = (key, env) => request.delete(_url(key, env))
