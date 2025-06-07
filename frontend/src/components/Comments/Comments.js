import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import "./Comments.css";

function Comments({ game, commentsList }) {
    const [comments, setComments] = useState([]);

    useEffect(() => {
        if (game) {
            setComments(commentsList);
        }
    }, [game, commentsList]);

    return (
        <div className="comments-container">
            {comments.map((comment) => (
                <div key={`comment-${comment.ident}`} className="comment-card">
                    <div className="comment-header">
                        <span className="comment-player">{comment.player}</span>
                        <span className="comment-date">
                            {new Date(comment.commentedOn).toLocaleString()}
                        </span>
                    </div>
                    <div className="comment-body">{comment.comment}</div>
                </div>
            ))}
        </div>
    );
}

Comments.propTypes = {
    game: PropTypes.string.isRequired,
    commentsList: PropTypes.arrayOf(
        PropTypes.shape({
            ident: PropTypes.number.isRequired,
            player: PropTypes.string.isRequired,
            comment: PropTypes.string.isRequired,
            commentedOn: PropTypes.string.isRequired,
        })
    ).isRequired,
};

export default Comments;