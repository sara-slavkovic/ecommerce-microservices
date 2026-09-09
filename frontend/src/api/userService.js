import { userApi } from './axiosInstances';

export function login(credentials) {
  return userApi.post('/users/login', credentials).then(res => res.data);
}

export function register(userData) {
  return userApi.post('/users/register', userData).then(res => res.data);
}

export function updateUser(id, dto) {
  return userApi.put(`/users/${id}`, dto).then(res => res.data);
}