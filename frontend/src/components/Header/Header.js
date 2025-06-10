import React from 'react';
import './Header.css';
import PropTypes from "prop-types";

function Header({ username, onLogout }) {
    return (
        <header className="header">
            <h1 className="header-title">GameStudio</h1>
            <div className="header-user-info">
                {username && (
                    <div>
                        <span className="username-text">{username}</span>
                        <button onClick={onLogout} className="btn btn-danger logout-button">
                            Logout
                        </button>
                    </div>
                )}
            </div>
        </header>
    );
}

Header.propTypes = {
    username: PropTypes.string,
    onLogout: PropTypes.func,
};

export default Header;