import React, { useState, useEffect } from 'react';
import { LogIn, Link, User, Trash2, Eye, Plus, LogOut, Info } from 'lucide-react';

// API Base URL
const API_BASE = 'http://localhost:5000';

// Auth Context
const AuthContext = React.createContext();

const ConfirmModal = ({ isOpen, message, onConfirm, onCancel }) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg shadow-lg max-w-sm w-full mx-4">
        <h3 className="text-lg font-semibold mb-4">{message}</h3>
        <div className="flex justify-end gap-2">
          <button
            onClick={onCancel}
            className="px-4 py-2 bg-gray-200 rounded-md hover:bg-gray-300"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700"
          >
            Delete
          </button>
        </div>
      </div>
    </div>
  );
};
// Auth Provider
const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('token'));

  useEffect(() => {
    if (token) {
      // Decode token to get user info (simplified)
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        setUser({
          id: payload.sub,
          login: payload.login || payload.name,
          role: payload.role || 'User'
        });
      } catch (error) {
        console.error('Invalid token', error);
        logout();
      }
    }
  }, [token]);

  const login = async (loginData) => {
    try {
      const response = await fetch(`${API_BASE}/api/users/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(loginData)
      });

      if (!response.ok) throw new Error('Login failed');
      
      const data = await response.json();
      setToken(data.token);
      localStorage.setItem('token', data.token);
      return true;
    } catch (error) {
      console.error('Login error:', error);
      return false;
    }
  };

  const register = async (registerData) => {
    try {
      const response = await fetch(`${API_BASE}/api/users/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(registerData)
      });

      if (!response.ok) throw new Error('Registration failed');
      
      const data = await response.json();
      setToken(data.token);
      localStorage.setItem('token', data.token);
      return true;
    } catch (error) {
      console.error('Registration error:', error);
      return false;
    }
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem('token');
  };

  return (
    <AuthContext.Provider value={{ user, token, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

// Custom hook for auth
const useAuth = () => {
  const context = React.useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
};

// API service
const apiService = {
  async request(endpoint, options = {}) {
    const token = localStorage.getItem('token');
    const config = {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { Authorization: `Bearer ${token}` }),
        ...options.headers
      },
      ...options
    };

    const response = await fetch(`${API_BASE}${endpoint}`, config);
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    if (response.status === 204) return null;
    return response.json();
  },

  async getAllUrls() {
    return this.request('/url-list');
  },

async createShortUrl(longUrl) {
  const response = await fetch(`${API_BASE}/shorten`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    },
    // Wrap the URL in quotes when stringifying
    body: JSON.stringify(longUrl)
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  // For 204 status, return success without trying to parse JSON
  if (response.status === 204) {
    return { success: true };
  }

  // Otherwise try to parse JSON response
  const contentType = response.headers.get('content-type');
  if (contentType && contentType.includes('application/json')) {
    return response.json();
  }

  return { success: true };
},

  async getUrlDetails(id) {
    return this.request(`/url/${id}/details`);
  },

  async deleteUrl(id) {
    return this.request(`/url/${id}`, { method: 'DELETE' });
  }
};

// Login Component
const LoginView = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [formData, setFormData] = useState({ login: '', password: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login, register } = useAuth();

  const handleSubmit = async () => {
    setLoading(true);
    setError('');

    try {
      const success = isLogin 
        ? await login(formData)
        : await register(formData);

      if (!success) {
        setError(isLogin ? 'Invalid credentials' : 'Registration failed');
      }
    } catch (error) {
      setError('An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
      <div className="bg-white p-8 rounded-lg shadow-md w-full max-w-md">
        <div className="text-center mb-6">
          <Link className="w-12 h-12 text-blue-600 mx-auto mb-4" />
          <h1 className="text-2xl font-bold text-gray-900">
            {isLogin ? 'Sign In' : 'Sign Up'}
          </h1>
        </div>

        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Login
            </label>
            <input
              type="text"
              name="login"
              value={formData.login}
              onChange={handleChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Password
            </label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              required
              maxLength={10}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {error && (
            <div className="text-red-600 text-sm text-center">{error}</div>
          )}

          <button
            onClick={handleSubmit}
            disabled={loading}
            className="w-full bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 disabled:opacity-50 flex items-center justify-center gap-2"
          >
            <LogIn className="w-4 h-4" />
            {loading ? 'Processing...' : (isLogin ? 'Sign In' : 'Sign Up')}
          </button>
        </div>

        <div className="text-center mt-4">
          <button
            onClick={() => setIsLogin(!isLogin)}
            className="text-blue-600 hover:underline"
          >
            {isLogin ? 'Need an account? Sign up' : 'Have an account? Sign in'}
          </button>
        </div>
      </div>
    </div>
  );
};

const UrlTableView = ({ onViewDetails }) => {
  const [urls, setUrls] = useState([]);
  const [newUrl, setNewUrl] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  const { user, token } = useAuth();
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [urlToDelete, setUrlToDelete] = useState(null);

  useEffect(() => {
    fetchUrls();
  }, []);

  const fetchUrls = async () => {
    try {
      const data = await apiService.getAllUrls();
      setUrls(data);
    } catch (error) {
      setError('Failed to fetch URLs');
    }
  };

  const handleAddUrl = async () => {
    if (!newUrl.trim()) return;

    setLoading(true);
    setError('');
    setSuccess('');

  try {
      await apiService.createShortUrl(newUrl);
      setSuccess('URL shortered successfully!');
      setNewUrl('');
      await fetchUrls();
    
  } catch (error) {
    console.error('Error details:', error); 
    if (error.message.includes('409')) {
      setError('URL already exists');
    } else {
      setError('failed to create short URL');
    }
  } finally {
    setLoading(false);
  }
  };

  const handleDelete = async (id, createdByUserId) => {
    if (!user || (user.role !== 'Admin' && user.id !== createdByUserId)) {
      setError('You can only delete your own URLs');
      return;
    }

    setUrlToDelete({ id, createdByUserId });
    setShowDeleteConfirm(true);
  };

  const confirmDelete = async () => {
    try {
      await apiService.deleteUrl(urlToDelete.id);
      setSuccess('URL deleted successfully!');
      await fetchUrls();
    } catch (error) {
      setError('Failed to delete URL');
    } finally {
      setShowDeleteConfirm(false);
      setUrlToDelete(null);
    }
  };

    const canDelete = (createdByUserId) => {
    return user && (user.role === 'Admin' || user.id === createdByUserId);
  };

  return (
    <div className="space-y-6">
      <ConfirmModal
      isOpen={showDeleteConfirm}
      message="Are you sure you want to delete this URL?"
      onConfirm={confirmDelete}
      onCancel={() => {
        setShowDeleteConfirm(false);
        setUrlToDelete(null);
      }}
    />
      {}
      {token && (
        <div className="bg-white p-6 rounded-lg shadow-md">
          <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
            <Plus className="w-5 h-5" />
            Add New URL
          </h2>
          
          <div className="flex gap-4">
            <input
              type="url"
              value={newUrl}
              onChange={(e) => setNewUrl(e.target.value)}
              placeholder="Enter URL to shorten"
              required
              className="flex-1 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <button
              onClick={handleAddUrl}
              disabled={loading}
              className="bg-blue-600 text-white px-6 py-2 rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {loading ? 'Creating...' : 'Shorten'}
            </button>
          </div>

          {error && <div className="text-red-600 text-sm mt-2">{error}</div>}
          {success && <div className="text-green-600 text-sm mt-2">{success}</div>}
        </div>
      )}

      {}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-200">
          <h2 className="text-xl font-semibold">Short URLs</h2>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Original URL
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Short URL
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {urls.map((url) => (
                <tr key={url.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="text-sm text-gray-900 truncate max-w-xs">
                      {url.longUrl}
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="text-sm text-blue-600">
                      {url.shortUrl}
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    <div className="flex items-center gap-2">
                      {token && (
                        <button
                          onClick={() => onViewDetails(url.id)}
                          className="text-blue-600 hover:text-blue-800"
                          title="View Details"
                        >
                          <Eye className="w-4 h-4" />
                        </button>
                      )}
                      {canDelete(url.createdByUserId) && (
                        <button
                          onClick={() => handleDelete(url.id, url.createdByUserId)}
                          className="text-red-600 hover:text-red-800"
                          title="Delete"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {urls.length === 0 && (
          <div className="text-center py-8 text-gray-500">
            No URLs found
          </div>
        )}
      </div>
    </div>
  );
};

const UrlDetailsView = ({ urlId, onBack }) => {
  const [urlDetails, setUrlDetails] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchUrlDetails = React.useCallback(async () => {
    try {
      setLoading(true);
      const data = await apiService.getUrlDetails(urlId);
      setUrlDetails(data);
    } catch (error) {
      setError('Failed to fetch URL details');
    } finally {
      setLoading(false);
    }
  }, [urlId]);

  useEffect(() => {
    fetchUrlDetails();
  }, [fetchUrlDetails]);


  if (loading) {
    return (
      <div className="flex items-center justify-center py-8">
        <div className="text-gray-500">Loading...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-white p-6 rounded-lg shadow-md">
        <div className="text-red-600 mb-4">{error}</div>
        <button
          onClick={onBack}
          className="bg-gray-600 text-white px-4 py-2 rounded-md hover:bg-gray-700"
        >
          Back to List
        </button>
      </div>
    );
  }

  return (
    <div className="bg-white p-6 rounded-lg shadow-md">
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-xl font-semibold flex items-center gap-2">
          <Info className="w-5 h-5" />
          URL Details
        </h2>
        <button
          onClick={onBack}
          className="bg-gray-600 text-white px-4 py-2 rounded-md hover:bg-gray-700"
        >
          Back to List
        </button>
      </div>

      {urlDetails && (
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              ID
            </label>
            <div className="text-sm text-gray-900">{urlDetails.id}</div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Original URL
            </label>
            <div className="text-sm text-gray-900 break-all">
              {urlDetails.longUrl}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Short URL
            </label>
            <div className="text-sm text-blue-600">
              {urlDetails.shortUrl}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Created By User ID
            </label>
            <div className="text-sm text-gray-900">{urlDetails.createdByUserId}</div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Created Date
            </label>
            <div className="text-sm text-gray-900">
              {new Date(urlDetails.timestamp).toLocaleString()}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

// About Component
const AboutView = () => {
  const [description, setDescription] = useState(
    "The algorithm works by hashing the entire URL using SHA256 and then taking the last 6 characters from this hash"
  );
  const [isEditing, setIsEditing] = useState(false);
  const [editDescription, setEditDescription] = useState(description);
  const { user } = useAuth();

  const handleSave = () => {
    setDescription(editDescription);
    setIsEditing(false);
    // Here you would typically save to the backend
  };

  const handleCancel = () => {
    setEditDescription(description);
    setIsEditing(false);
  };

  return (
    <div className="bg-white p-6 rounded-lg shadow-md">
      <h2 className="text-2xl font-bold mb-6">About URL Shortener</h2>
      
      {isEditing ? (
        <div className="space-y-4">
          <textarea
            value={editDescription}
            onChange={(e) => setEditDescription(e.target.value)}
            className="w-full h-32 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          <div className="flex gap-2">
            <button
              onClick={handleSave}
              className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700"
            >
              Save
            </button>
            <button
              onClick={handleCancel}
              className="bg-gray-600 text-white px-4 py-2 rounded-md hover:bg-gray-700"
            >
              Cancel
            </button>
          </div>
        </div>
      ) : (
        <div>
          <p className="text-gray-700 mb-4">{description}</p>
          {user?.role === 'Admin' && (
            <button
              onClick={() => setIsEditing(true)}
              className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700"
            >
              Edit Description
            </button>
          )}
        </div>
      )}
    </div>
  );
};

// Navigation Component
const Navigation = ({ currentView, setCurrentView }) => {
  const { user, logout } = useAuth();

  return (
    <nav className="bg-white shadow-md mb-6">
      <div className="max-w-7xl mx-auto px-4">
        <div className="flex justify-between items-center py-4">
          <div className="flex items-center gap-2">
            <Link className="w-8 h-8 text-blue-600" />
            <h1 className="text-xl font-bold text-gray-900">URL Shortener</h1>
          </div>

          <div className="flex items-center gap-4">
            <button
              onClick={() => setCurrentView('table')}
              className={`px-4 py-2 rounded-md ${
                currentView === 'table'
                  ? 'bg-blue-600 text-white'
                  : 'text-gray-600 hover:bg-gray-100'
              }`}
            >
              URLs
            </button>

            <button
              onClick={() => setCurrentView('about')}
              className={`px-4 py-2 rounded-md ${
                currentView === 'about'
                  ? 'bg-blue-600 text-white'
                  : 'text-gray-600 hover:bg-gray-100'
              }`}
            >
              About
            </button>

            {user && (
              <div className="flex items-center gap-2">
                <div className="flex items-center gap-1 text-gray-600">
                  <User className="w-4 h-4" />
                  <span>{user.login} ({user.role})</span>
                </div>
                <button
                  onClick={logout}
                  className="text-red-600 hover:bg-red-50 px-2 py-1 rounded"
                  title="Logout"
                >
                  <LogOut className="w-4 h-4" />
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};

// Main App Component
const App = () => {
  const [currentView, setCurrentView] = useState('table');
  const [selectedUrlId, setSelectedUrlId] = useState(null);
  const { user } = useAuth();

  const handleViewDetails = (urlId) => {
    setSelectedUrlId(urlId);
    setCurrentView('details');
  };

  const handleBackToTable = () => {
    setSelectedUrlId(null);
    setCurrentView('table');
  };

  if (!user) {
    return <LoginView />;
  }

  return (
    <div className="min-h-screen bg-gray-100">
      <Navigation currentView={currentView} setCurrentView={setCurrentView} />
      
      <div className="max-w-7xl mx-auto px-4">
        {currentView === 'table' && (
          <UrlTableView onViewDetails={handleViewDetails} />
        )}
        
        {currentView === 'details' && selectedUrlId && (
          <UrlDetailsView 
            urlId={selectedUrlId} 
            onBack={handleBackToTable} 
          />
        )}
        
        {currentView === 'about' && <AboutView />}
      </div>
    </div>
  );
};

// Root App with Auth Provider
const URLShortenerApp = () => {
  return (
    <AuthProvider>
      <App />
    </AuthProvider>
  );
};

export default URLShortenerApp;