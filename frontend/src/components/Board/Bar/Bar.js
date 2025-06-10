import PropTypes from "prop-types";
import {useEffect, useState} from "react";
import BarPart from "./BarPart";
import React from "react";


function Bar({ barData: initialBarData, mirrored }) {
    const [barData, setBarData] = useState(initialBarData);

    useEffect(() => {
        setBarData(initialBarData);
    }, [initialBarData]);

    let topBarData;
    let bottomBarData;

    if (mirrored) {
        topBarData = {
            checkersCount: barData.blackCheckersCount,
            checkersColor: 'black',
            selected: barData.selectedForPlayer2,
        }

        bottomBarData = {
            checkersCount: barData.whiteCheckersCount,
            checkersColor: 'white',
            selected: barData.selectedForPlayer1,
        }
    } else {
        topBarData = {
            checkersCount: barData.whiteCheckersCount,
            checkersColor: 'white',
            selected: barData.selectedForPlayer1,
        }

        bottomBarData = {
            checkersCount: barData.blackCheckersCount,
            checkersColor: 'black',
            selected: barData.selectedForPlayer2,
        }
    }

    return (
        <div className="bar-container">
            <BarPart position={'top'} barPartData={topBarData} />
            <BarPart position={'bottom'} barPartData={bottomBarData} />
        </div>
    )
}

Bar.propTypes = {
    barData: PropTypes.shape({
        whiteCheckersCount: PropTypes.number.isRequired,
        blackCheckersCount: PropTypes.number.isRequired,
        selectedForPlayer1: PropTypes.bool.isRequired,
        selectedForPlayer2: PropTypes.bool.isRequired,
    }).isRequired,
    mirrored: PropTypes.bool,
}

export default Bar;