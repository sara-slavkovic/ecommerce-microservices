import { inventoryApi } from './axiosInstances';

export function restockInventory(productId, quantity) {
  return inventoryApi.post('/inventories/restock', { productId, quantity }).then(res => res.data);
}