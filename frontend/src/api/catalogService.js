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

export function uploadProductImage(file, productName) {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('productName', productName);
  return catalogApi.post('/products/upload-image', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(res => res.data.imageUrl);
}

export function createProduct(dto) {
  return catalogApi.post('/products', dto).then(res => res.data);
}

export function updateProduct(id, dto) {
  return catalogApi.put(`/products/${id}`, dto).then(res => res.data);
}

export function deleteProduct(id) {
  return catalogApi.delete(`/products/${id}`);
}