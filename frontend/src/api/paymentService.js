import { paymentApi } from './axiosInstances';

export function initiatePayment(orderId, amount) {
  return paymentApi.post('/payments', { orderId, amount }).then(res => res.data);
}

export function getPaymentByOrderId(orderId) {
  return paymentApi.get(`/payments/order/${orderId}`).then(res => res.data);
}