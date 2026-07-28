import axios from 'axios';

const PORTS = {
  USER: '7082',
  CATALOG: '7038',
  CART: '7252',
  ORDER: '7015',
  PAYMENT: '7213'
};

export const userApi = axios.create({ baseURL: `https://localhost:${PORTS.USER}/api` });
export const catalogApi = axios.create({ baseURL: `https://localhost:${PORTS.CATALOG}/api` });
export const cartApi = axios.create({ baseURL: `https://localhost:${PORTS.CART}/api` });
export const orderApi = axios.create({ baseURL: `https://localhost:${PORTS.ORDER}/api` });
export const paymentApi = axios.create({ baseURL: `https://localhost:${PORTS.PAYMENT}/api` });