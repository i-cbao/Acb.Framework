import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

/* Layout */
import Layout from '@/views/layout/Layout'

export const constantRouterMap = [{
  path: '/login',
  component: () =>
      import('@/views/login/index'),
  hidden: true
},
{
  path: '/authredirect',
  component: () =>
      import('@/views/login/authredirect'),
  hidden: true
},
{
  path: '/404',
  component: () =>
      import('@/views/errorPage/404'),
  hidden: true
},
{
  path: '/401',
  component: () =>
      import('@/views/errorPage/401'),
  hidden: true
},
{
  path: '',
  component: Layout,
  redirect: '/config/index',
  hidden: true
}
]

export default new Router({
  // mode: 'history', // require service support
  scrollBehavior: () => ({
    y: 0
  }),
  routes: constantRouterMap
})

// 配置中心
export const asyncRouterMap = [{
  path: '/config',
  component: Layout,
  redirect: 'noredirect',
  children: [{
    path: 'index',
    component: () =>
        import('@/views/config/index'),
    name: 'Config',
    meta: {
      title: 'config',
      icon: 'table'
    }
  }]
},
{
  path: '/job',
  component: Layout,
  redirect: 'noredirect',
  children: [{
    path: 'index',
    component: () =>
        import('@/views/jobs/index'),
    name: 'Job',
    meta: {
      title: 'job',
      icon: 'guide'
    }
  }]
},
{
  path: '*',
  redirect: '/404',
  hidden: true
}
]
