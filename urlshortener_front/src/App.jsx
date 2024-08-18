import React from 'react';
import URLShortenerForm from './components/URLShortenerForm';
import ShortenedUrlList from './components/ShortenedUrlList';
import './App.css';

function App() {
    return (
        <div className="app-container container">
            <URLShortenerForm />
            <ShortenedUrlList />
        </div>
    );
}

export default App;
