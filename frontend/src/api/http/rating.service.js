import gsAxios from "./axiosConfig";

export const fetchAverageRating = game => {
    return gsAxios.get("/rating/" + game)
}

export const fetchUserRating = (game, username) => {
    return gsAxios.get("/rating/" + game + "/" + username)
}

export const setRating = (game, rating) => {
    return gsAxios.put("/rating/" + game, {
        rating: rating
    });
};