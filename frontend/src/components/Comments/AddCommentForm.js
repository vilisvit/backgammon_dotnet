import React, { useState } from 'react';
import { addComment } from '../../api/http/comment.service';
import { Form, Button } from 'react-bootstrap';
import PropTypes from "prop-types";
import './AddCommentForm.css';

function AddCommentForm({ game, onCommentAdded }) {
    const [comment, setComment] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = (e) => {
        e.preventDefault();
        if (!comment) {
            setError('Comment cannot be empty.');
            return;
        }

        addComment(game, comment)
            .then(() => {
                if (onCommentAdded) {
                    onCommentAdded();
                }
            })
            .catch((err) => {
                console.error('Error adding comment:', err);
                setError('Failed to add comment. Please try again.');
            });
    };

    return (
        <div className="add-comment-container">
            <Form onSubmit={handleSubmit} className="add-comment-form">
                <Form.Group controlId="commentText" className="form-group">
                    <Form.Label className="form-label">Add a Comment</Form.Label>
                    <Form.Control
                        as="textarea"
                        rows={3}
                        placeholder="Enter your comment"
                        value={comment}
                        onChange={(e) => setComment(e.target.value)}
                        className="form-control"
                    />
                </Form.Group>
                {error && <p className="error-message">{error}</p>}
                <Button variant="primary" type="submit" className="submit-button">
                    Submit
                </Button>
            </Form>
        </div>
    );
}

AddCommentForm.propTypes = {
    game: PropTypes.string.isRequired,
    onCommentAdded: PropTypes.func,
};

export default AddCommentForm;