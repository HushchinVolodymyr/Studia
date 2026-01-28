import React from 'react';
import Header from "@/components/home-page/header";
import MainBlock from "@/components/home-page/main-block";

const HomePage = () => {
    return (
        <div className={"flex flex-col gap-2"}>
            <Header />
            <MainBlock/>
        </div>
    );
};

export default HomePage;