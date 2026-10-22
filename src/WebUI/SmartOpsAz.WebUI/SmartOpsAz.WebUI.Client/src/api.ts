import axios from 'axios';
export const api = axios.create({baseURL:'/api',timeout:15000});
api.interceptors.request.use(config=>{const token=sessionStorage.getItem('smartops_token');if(token)config.headers.Authorization=`Bearer ${token}`;return config;});
export function errorText(e:unknown){if(axios.isAxiosError(e)){const data=e.response?.data;return data?.detail||data?.message||data?.error||(Array.isArray(data?.errors)?data.errors.join(', '):null)||e.message;}return e instanceof Error?e.message:'Unexpected error';}
export const inventoryStatus:Record<number,string>={0:'Available',1:'Available',2:'In Use',3:'Maintenance',4:'Offline',5:'Retired'};
export const deviceStatus:Record<number,string>={1:'Online',2:'Offline',3:'Warning',4:'Critical',5:'Maintenance'}; 