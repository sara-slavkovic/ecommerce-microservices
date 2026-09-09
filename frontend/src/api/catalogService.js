import { catalogApi, PORTS } from './axiosInstances';

export function getAllProducts() {
  return catalogApi.get('/products').then(res => res.data);
}

export function getAllCategories() {
  return catalogApi.get('/categories').then(res => res.data);
}

export function getProductById(id) {
  return catalogApi.get(`/products/${id}`).then(res => res.data);
}

export function getImageUrl(path) {
  if (!path || !path.startsWith('images')) return 'https://via.placeholder.com/230?text=No+Image';
  return `https://localhost:${PORTS.CATALOG}/${path}`;
}