import { orderApi } from './axiosInstances';

export function createOrder(dto) {
  return orderApi.post('/orders', dto).then(res => res.data);
}

export function getOrdersByUserId(userId) {
  return orderApi.get(`/orders/user/${userId}`).then(res => res.data);
}

export function getOrderById(id) {
  return orderApi.get(`/orders/${id}`).then(res => res.data);
}