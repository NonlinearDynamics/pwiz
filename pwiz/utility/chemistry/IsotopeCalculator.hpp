//
// $Id$ 
//
//
// Original author: Darren Kessner <darren@proteowizard.org>
//
// Copyright 2006 Louis Warschaw Prostate Cancer Center
//   Cedars Sinai Medical Center, Los Angeles, California  90048
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
//


#ifndef _ISOTOPECALCULATOR_HPP_
#define _ISOTOPECALCULATOR_HPP_


#include "pwiz/utility/misc/Export.hpp"
#include "Chemistry.hpp"
#include <memory>


namespace pwiz {
namespace chemistry {


class PWIZ_API_DECL IsotopeCalculator
{
    public:
    
    IsotopeCalculator(double abundanceCutoff, double massPrecision);
    ~IsotopeCalculator();

    enum PWIZ_API_DECL NormalizationFlags
    {
        NormalizeMass = 0x01,       // shift masses -> monoisotopic_mass == 0
        NormalizeAbundance = 0x02   // scale abundances -> sum(abundance[i]^2) == 1 
    };
    
    MassDistribution distribution(const Formula& formula,
                                             int chargeState = 0,
                                             int normalization = 0) const;
    private:
    class Impl;
    std::unique_ptr<Impl> impl_;

    // no copying
    IsotopeCalculator(const IsotopeCalculator&);
    IsotopeCalculator& operator=(const IsotopeCalculator);
};


} // namespace chemistry
} // namespace pwiz


#endif // _ISOTOPECALCULATOR_HPP_
