import gsAxios from "./axiosConfig";

export const fetchComments = game => {
    return gsAxios.get("/comment/" + game)
}

export const addComment = (game, comment) => {
    return gsAxios.post("/comment/" + game, {
        comment: comment,
    });
};