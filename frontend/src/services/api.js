import { httpClient } from './httpClient.js';

const API_BASE_URL = import.meta.env.PROD ? '/api' : 'http://localhost:5000/api';

export const api = {
  async submitForm(formType, data) {
    const request = { formType, data };
    return httpClient.post(`${API_BASE_URL}/submissions`, request);
  },

  async getSubmissions(search) {
    const url = search 
      ? `${API_BASE_URL}/submissions?search=${encodeURIComponent(search)}`
      : `${API_BASE_URL}/submissions`;
    
    return httpClient.get(url);
  },

  async getSubmissionById(id) {
    return httpClient.get(`${API_BASE_URL}/submissions/${id}`);
  },
};


