// Created Date: 16/08/2022
// Original Author: Teodor Vecerdi
// 
// Copyright (c) 2022 ATiStudios. All rights reserved.

namespace ORST.Foundation.Random {
    public interface IRandomProvider {
        /// <summary>
        /// Seed used to generate values
        /// </summary>
        uint Seed { get; set; }

        /// <summary>
        /// Returns a random integer based on the seed and <paramref name="iterations"/>.
        /// </summary>
        int GetInt(uint iterations);

        /// <summary>
        /// Returns a random float between 0 and 1 based on the seed and <paramref name="iterations"/>.
        /// </summary>
        /// <param name="iterations"></param>
        /// <returns></returns>
        float GetFloat(uint iterations);
    }
}