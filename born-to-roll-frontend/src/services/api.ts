import axios from 'axios';

const API = axios.create({
  baseURL: 'http://localhost:5000/api',
});

API.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export const authAPI = {
  login: (email: string, password: string) => 
    API.post('/auth/login', { email, password }),
  register: (email: string, password: string, name: string) => 
    API.post('/auth/register', { email, password, name }),
  me: () => API.get('/auth/me'),
  forgotPassword: (email: string) =>
    API.post('/auth/forgot-password', { email }),
  resetPassword: (token: string, newPassword: string) =>
    API.post('/auth/reset-password', { token, newPassword }),
};

export default API;