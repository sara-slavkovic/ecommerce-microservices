import { catalogApi } from './axiosInstances';

export function getAllProducts() {
  return catalogApi.get('/products').then(res => res.data);
}