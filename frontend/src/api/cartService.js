import { cartApi } from './axiosInstances';

export function getCartByUserId(userId) {
  return cartApi.get(`/carts/user/${userId}`).then(res => res.data);
}

export function addItemToCart(userId, productId, quantity = 1) {
  return cartApi.post('/carts/items', { userId, productId, quantity }).then(res => res.data);
}

export function updateCartItemQuantity(userId, productId, quantity) {
  return cartApi.put('/carts/items', { userId, productId, quantity }).then(res => res.data);
}

export function removeCartItem(userId, productId) {
  return cartApi.delete(`/carts/user/${userId}/items/${productId}`).then(res => res.data);
}