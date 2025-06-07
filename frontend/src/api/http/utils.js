export const formatDate = (date) => {
    let dateString = date.toISOString();
    dateString = dateString.slice(0, dateString.length - 1) + "+00:00";
    return dateString;
}