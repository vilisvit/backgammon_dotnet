import gsAxios from "./axiosConfig";

export const fetchTopScores = game => {
    return gsAxios.get("/score/" + game);
}