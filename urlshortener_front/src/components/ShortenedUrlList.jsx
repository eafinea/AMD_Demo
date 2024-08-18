import React, { useState, useEffect } from 'react';
import { getShortenedUrls, deleteShortenedUrl, refreshUrls } from '../services/apiService';

function ShortenedUrlList() {
    const [urls, setUrls] = useState([]);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [sortOption, setSortOption] = useState('id');
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        fetchUrls();
    }, [page, sortOption]);

    const fetchUrls = async () => {
        try {
            const response = await getShortenedUrls(page, 10, sortOption);  // Fetch data without sorting
            const sortedData = sortUrls(response.data.data);  // Sort data on frontend
            setUrls(sortedData);
            setTotalPages(response.data.totalPages);
        } catch (error) {
            console.error('Error fetching URLs:', error);
        }
    };

    const sortUrls = (data) => {
        return data.sort((a, b) => {
            // Extract domain and path, ignoring query parameters
            const extractMainPart = (url) => {
                const urlObj = new URL(url);
                return `${urlObj.hostname}${urlObj.pathname}`;
            };

            if (sortOption === 'id') {
                return a.id > b.id ? 1 : -1;
            } else if (sortOption === 'createdAt') {
                return new Date(a.createdDate) > new Date(b.createdDate) ? 1 : -1;
            } else if (sortOption === 'originalUrl') {
                const urlA = extractMainPart(a.originalUrl);
                const urlB = extractMainPart(b.originalUrl);
                return urlA.localeCompare(urlB);  // Compare based on the extracted main part of the URL
            }
            return 0;
        });
    };


    const handleSortChange = (e) => {
        setSortOption(e.target.value);
        const sortedData = sortUrls(urls);  // Sort data again when sort option changes
        setUrls(sortedData);
    };

    const handleRefresh = async () => {
        try {
            await refreshUrls();  // Correct function reference
            fetchUrls();  // Fetch the updated list of URLs
        } catch (error) {
            console.error('Error refreshing URLs:', error);
        }
    };

    const handleDelete = async (shortCode) => {
        try {
            await deleteShortenedUrl(shortCode);
            fetchUrls();
        } catch (error) {
            console.error('Error deleting URL:', error);
        }
    };

    return (
        <div className="container mt-5">
            <h2>Shortened URLs</h2>
            <div className="mb-3">
                <button onClick={handleRefresh} className="btn btn-secondary mr-2">Refresh</button>
                <select value={sortOption} onChange={handleSortChange} className="form-control w-auto d-inline-block">
                    <option value="id">Sort by ID</option>
                    <option value="createdAt">Sort by Creation Date</option>
                    <option value="originalUrl">Sort by Original URL</option>
                </select>
            </div>

            <table className="table table-dark table-striped">
                <thead>
                    <tr>
                        <th>Shortened URL</th>
                        <th>Original URL</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {urls.map((url) => (
                        <tr key={url.shortCode}>
                            <td>
                                <a href={`https://localhost:8888/${url.shortCode}`} target="_blank" rel="noopener noreferrer">
                                    {url.shortCode}
                                </a>
                            </td>
                            <td>{url.originalUrl}</td>
                            <td>
                                <button onClick={() => handleDelete(url.shortCode)} className="btn btn-danger">Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <div>
                <button onClick={() => setPage(page - 1)} className="btn btn-primary mr-2" disabled={page === 1}>Previous</button>
                <button onClick={() => setPage(page + 1)} className="btn btn-primary" disabled={page === totalPages}>Next</button>
            </div>
        </div>
    );
}

export default ShortenedUrlList;