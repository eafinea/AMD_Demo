import React, { useState } from 'react';
import { shortenUrl } from '../services/apiService';

function URLShortenerForm() {
    const [url, setUrl] = useState('');
    const [expirationOption, setExpirationOption] = useState('1 day');
    const [shortCode, setShortCode] = useState('');
    const [error, setError] = useState('');

    const isValidUrl = (url) => {
        // Regular expression to match the desired URL formats
        const pattern = /^(https?:\/\/)?(www\.)?[a-zA-Z0-9-]+\.[a-zA-Z]{2,}(\.[a-zA-Z]{2,})?\/?/;
        return pattern.test(url);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!isValidUrl(url)) {
            setError('Please enter a valid URL.');
            return;
        }

        try {
            const response = await shortenUrl(url, expirationOption);
            setShortCode(response.data);
            setError('');  // Clear any previous error messages
        } catch (error) {
            console.error('Error shortening URL:', error);
            setError('Failed to shorten the URL.');
        }
    };

    return (
        <div className="container d-flex justify-content-center">
            <div className="col-md-8">
                <h2 className="text-center my-4">Shorten URL</h2>
                <form onSubmit={handleSubmit} className="form-inline justify-content-center">
                    <input
                        type="text"
                        value={url}
                        onChange={(e) => setUrl(e.target.value)}
                        className="form-control mr-2 mb-2"
                        placeholder="Enter URL"
                        required
                    />
                    <select
                        value={expirationOption}
                        onChange={(e) => setExpirationOption(e.target.value)}
                        className="form-control mr-2 mb-2"
                    >
                        <option value="1 day">1 Day</option>
                        <option value="1 week">1 Week</option>
                        <option value="indefinite">Indefinite</option>
                    </select>
                    <button type="submit" className="btn btn-primary mb-2">Shorten</button>
                </form>
                {error && <p style={{ color: 'red' }}>{error}</p>}
                {shortCode && (
                    <p>
                        Shortened URL:{" "}
                        <a href={`https://localhost:8888/${shortCode}`} target="_blank" rel="noopener noreferrer">
                            https://localhost:8888/{shortCode}
                        </a>
                    </p>
                )}
            </div>
        </div>
    );
}

export default URLShortenerForm;
