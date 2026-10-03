import axios from 'axios';

const GATEWAY_URL = 'https://localhost:7100/api';

export const userApi = axios.create({ baseURL: GATEWAY_URL });
export const catalogApi = axios.create({ baseURL: GATEWAY_URL });
export const cartApi = axios.create({ baseURL: GATEWAY_URL });
export const orderApi = axios.create({ baseURL: GATEWAY_URL });
export const paymentApi = axios.create({ baseURL: GATEWAY_URL });
export const inventoryApi = axios.create({ baseURL: GATEWAY_URL });