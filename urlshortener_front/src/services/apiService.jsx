import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:8888',
});

export const shortenUrl = (originalUrl, expirationOption = '1 day') =>
    api.post('/Urls/shorten', { originalUrl, expirationOption });

export const getShortenedUrls = (page = 1, pageSize = 10, sortBy = 'id') =>
    api.get('/Urls', { params: { page, pageSize, sortBy } });

export const deleteShortenedUrl = (shortCode) =>
    api.delete(`/Urls/${shortCode}`);

export const refreshUrls = () =>
    api.post('/Urls/refresh');

export const redirectToOriginalUrl = (shortCode) =>
    api.get(`/${shortCode}`);
