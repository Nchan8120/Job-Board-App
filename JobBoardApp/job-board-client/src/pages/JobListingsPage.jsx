import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import api from '../services/api';

export default function JobListingsPage() {
  const [jobs, setJobs] = useState([]);
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);

  const fetchJobs = async () => {
    setLoading(true);
    try {
      const res = await api.get('/jobs', {
        params: { search, page, pageSize: 10 }
      });
      setJobs(res.data.items);
      setTotalPages(res.data.totalPages);
    } catch {
      console.error('Failed to fetch jobs');
    } finally {
      setLoading(false);
    }
  };

  // Fetch jobs when page changes
  useEffect(() => {
    fetchJobs();
  }, [page]);

  // Reset to page 1 and fetch when search changes
  const handleSearch = (e) => {
    e.preventDefault();
    setPage(1);
    fetchJobs();
  };

  return (
    <div>
      <h1 style={{ marginBottom: '1.5rem' }}>Job listings</h1>

      {/* Search bar */}
      <form onSubmit={handleSearch} style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem' }}>
        <input
          type="text"
          placeholder="Search jobs..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          style={{ flex: 1 }}
        />
        <button type="submit" className="btn-primary">Search</button>
        {search && (
          <button
            type="button"
            className="btn-secondary"
            onClick={() => { setSearch(''); setPage(1); fetchJobs(); }}
          >
            Clear
          </button>
        )}
      </form>

      {/* Job list */}
      {loading ? (
        <p>Loading...</p>
      ) : jobs.length === 0 ? (
        <div className="card" style={{ textAlign: 'center', color: '#666' }}>
          <p>No jobs found.</p>
        </div>
      ) : (
        jobs.map(job => (
          <Link to={`/jobs/${job.id}`} key={job.id} style={{ textDecoration: 'none', color: 'inherit' }}>
            <div className="card" style={{ cursor: 'pointer' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                <h3 style={{ marginBottom: '0.5rem' }}>{job.summary}</h3>
                <span style={{
                  background: '#eff6ff',
                  color: '#2563eb',
                  padding: '0.2rem 0.6rem',
                  borderRadius: '999px',
                  fontSize: '0.8rem',
                  whiteSpace: 'nowrap'
                }}>
                  {job.interestCount} interested
                </span>
              </div>
              <p style={{ color: '#555', marginBottom: '0.75rem', fontSize: '0.95rem' }}>
                {job.body.length > 120 ? job.body.substring(0, 120) + '...' : job.body}
              </p>
              <div style={{ fontSize: '0.8rem', color: '#888' }}>
                Posted by <strong>{job.postedBy}</strong> · {new Date(job.postedDate).toLocaleDateString()}
              </div>
            </div>
          </Link>
        ))
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <div style={{ display: 'flex', justifyContent: 'center', gap: '0.5rem', marginTop: '1.5rem' }}>
          <button
            className="btn-secondary"
            onClick={() => setPage(p => p - 1)}
            disabled={page === 1}
          >
            Previous
          </button>
          <span style={{ padding: '0.5rem 1rem', fontSize: '0.9rem' }}>
            Page {page} of {totalPages}
          </span>
          <button
            className="btn-secondary"
            onClick={() => setPage(p => p + 1)}
            disabled={page === totalPages}
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
}