namespace ML {
    public static class AI_Brain {
        public static double[] Score(double[] input) {
            double[] var0;
            if (input[0] <= 6.026570081710815) {
                if (input[1] <= 1.0563380420207977) {
                    if (input[2] <= 1.5714285969734192) {
                        if (input[2] <= 1.0215947031974792) {
                            var0 = new double[3] {0.0, 0.09090909090909091, 0.9090909090909091};
                        } else {
                            var0 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        if (input[2] <= 1.7516611218452454) {
                            var0 = new double[3] {0.8666666666666667, 0.0, 0.13333333333333333};
                        } else {
                            var0 = new double[3] {0.5714285714285714, 0.0, 0.42857142857142855};
                        }
                    }
                } else {
                    var0 = new double[3] {0.0, 1.0, 0.0};
                }
            } else {
                if (input[0] <= 8.610305786132812) {
                    var0 = new double[3] {1.0, 0.0, 0.0};
                } else {
                    if (input[0] <= 8.88164234161377) {
                        var0 = new double[3] {0.0, 0.0, 1.0};
                    } else {
                        var0 = new double[3] {1.0, 0.0, 0.0};
                    }
                }
            }
            double[] var1;
            if (input[2] <= 6.292358875274658) {
                if (input[2] <= 1.514950156211853) {
                    if (input[2] <= 1.1104651093482971) {
                        if (input[2] <= 1.0490033030509949) {
                            var1 = new double[3] {0.0, 0.4358974358974359, 0.5641025641025641};
                        } else {
                            var1 = new double[3] {0.0, 0.6818181818181818, 0.3181818181818182};
                        }
                    } else {
                        if (input[1] <= 1.7183098495006561) {
                            var1 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var1 = new double[3] {0.0, 1.0, 0.0};
                        }
                    }
                } else {
                    if (input[0] <= 2.708534598350525) {
                        if (input[2] <= 1.67524915933609) {
                            var1 = new double[3] {0.26666666666666666, 0.7333333333333333, 0.0};
                        } else {
                            var1 = new double[3] {0.3902439024390244, 0.36585365853658536, 0.24390243902439024};
                        }
                    } else {
                        if (input[2] <= 2.3779069781303406) {
                            var1 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var1 = new double[3] {0.0, 0.9722222222222222, 0.027777777777777776};
                        }
                    }
                }
            } else {
                if (input[2] <= 16.260797023773193) {
                    var1 = new double[3] {1.0, 0.0, 0.0};
                } else {
                    if (input[1] <= 0.6091549396514893) {
                        if (input[0] <= 107.53542518615723) {
                            var1 = new double[3] {1.0, 0.0, 0.0};
                        } else {
                            var1 = new double[3] {0.0, 1.0, 0.0};
                        }
                    } else {
                        var1 = new double[3] {1.0, 0.0, 0.0};
                    }
                }
            }
            double[] var2;
            if (input[1] <= 1.0563380420207977) {
                if (input[1] <= 0.47535212337970734) {
                    if (input[1] <= 0.42957746982574463) {
                        if (input[0] <= 1.5458937287330627) {
                            var2 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var2 = new double[3] {0.9310344827586207, 0.0, 0.06896551724137931};
                        }
                    } else {
                        var2 = new double[3] {1.0, 0.0, 0.0};
                    }
                } else {
                    if (input[2] <= 1.6121262311935425) {
                        if (input[2] <= 1.0232558250427246) {
                            var2 = new double[3] {0.0, 0.1111111111111111, 0.8888888888888888};
                        } else {
                            var2 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        if (input[2] <= 5.240863800048828) {
                            var2 = new double[3] {0.5, 0.0, 0.5};
                        } else {
                            var2 = new double[3] {0.9555555555555556, 0.044444444444444446, 0.0};
                        }
                    }
                }
            } else {
                var2 = new double[3] {0.0, 1.0, 0.0};
            }
            double[] var3;
            if (input[2] <= 6.196843862533569) {
                if (input[1] <= 1.0563380420207977) {
                    if (input[0] <= 1.5249597430229187) {
                        var3 = new double[3] {0.0, 0.0, 1.0};
                    } else {
                        if (input[0] <= 1.6658614873886108) {
                            var3 = new double[3] {0.3333333333333333, 0.0, 0.6666666666666666};
                        } else {
                            var3 = new double[3] {0.7083333333333334, 0.041666666666666664, 0.25};
                        }
                    }
                } else {
                    var3 = new double[3] {0.0, 1.0, 0.0};
                }
            } else {
                if (input[1] <= 0.48591549694538116) {
                    if (input[0] <= 107.35507011413574) {
                        var3 = new double[3] {1.0, 0.0, 0.0};
                    } else {
                        var3 = new double[3] {0.0, 1.0, 0.0};
                    }
                } else {
                    var3 = new double[3] {1.0, 0.0, 0.0};
                }
            }
            double[] var4;
            if (input[2] <= 6.196843862533569) {
                if (input[1] <= 1.0563380420207977) {
                    if (input[0] <= 1.5579710602760315) {
                        if (input[0] <= 1.5249597430229187) {
                            var4 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var4 = new double[3] {0.25, 0.0, 0.75};
                        }
                    } else {
                        if (input[0] <= 2.111916184425354) {
                            var4 = new double[3] {0.4166666666666667, 0.0, 0.5833333333333334};
                        } else {
                            var4 = new double[3] {0.8571428571428571, 0.0, 0.14285714285714285};
                        }
                    }
                } else {
                    var4 = new double[3] {0.0, 1.0, 0.0};
                }
            } else {
                if (input[0] <= 108.33655166625977) {
                    var4 = new double[3] {1.0, 0.0, 0.0};
                } else {
                    var4 = new double[3] {0.0, 1.0, 0.0};
                }
            }
            double[] var5;
            if (input[1] <= 1.4049296081066132) {
                if (input[1] <= 0.47535212337970734) {
                    if (input[1] <= 0.4154929518699646) {
                        if (input[1] <= 0.36267605423927307) {
                            var5 = new double[3] {1.0, 0.0, 0.0};
                        } else {
                            var5 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        if (input[1] <= 0.43661971390247345) {
                            var5 = new double[3] {0.75, 0.0, 0.25};
                        } else {
                            var5 = new double[3] {1.0, 0.0, 0.0};
                        }
                    }
                } else {
                    if (input[2] <= 4.490033388137817) {
                        if (input[1] <= 0.75) {
                            var5 = new double[3] {0.0547945205479452, 0.0, 0.9452054794520548};
                        } else {
                            var5 = new double[3] {0.1951219512195122, 0.0, 0.8048780487804879};
                        }
                    } else {
                        if (input[0] <= 109.9975814819336) {
                            var5 = new double[3] {1.0, 0.0, 0.0};
                        } else {
                            var5 = new double[3] {0.0, 1.0, 0.0};
                        }
                    }
                }
            } else {
                var5 = new double[3] {0.0, 1.0, 0.0};
            }
            double[] var6;
            if (input[1] <= 1.0563380420207977) {
                if (input[2] <= 1.6121262311935425) {
                    if (input[2] <= 1.5058139562606812) {
                        if (input[1] <= 0.5950704216957092) {
                            var6 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var6 = new double[3] {0.0, 0.04, 0.96};
                        }
                    } else {
                        if (input[2] <= 1.580564796924591) {
                            var6 = new double[3] {1.0, 0.0, 0.0};
                        } else {
                            var6 = new double[3] {0.0, 0.0, 1.0};
                        }
                    }
                } else {
                    if (input[1] <= 0.7605633735656738) {
                        if (input[1] <= 0.6760563552379608) {
                            var6 = new double[3] {0.813953488372093, 0.0, 0.18604651162790697};
                        } else {
                            var6 = new double[3] {0.125, 0.125, 0.75};
                        }
                    } else {
                        if (input[2] <= 4.382890462875366) {
                            var6 = new double[3] {0.6, 0.0, 0.4};
                        } else {
                            var6 = new double[3] {1.0, 0.0, 0.0};
                        }
                    }
                }
            } else {
                var6 = new double[3] {0.0, 1.0, 0.0};
            }
            double[] var7;
            if (input[0] <= 6.026570081710815) {
                if (input[1] <= 1.0563380420207977) {
                    if (input[2] <= 1.5714285969734192) {
                        if (input[1] <= 0.6021126806735992) {
                            var7 = new double[3] {0.0, 0.05128205128205128, 0.9487179487179487};
                        } else {
                            var7 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        if (input[0] <= 2.1698873043060303) {
                            var7 = new double[3] {0.42105263157894735, 0.0, 0.5789473684210527};
                        } else {
                            var7 = new double[3] {1.0, 0.0, 0.0};
                        }
                    }
                } else {
                    var7 = new double[3] {0.0, 1.0, 0.0};
                }
            } else {
                if (input[2] <= 6.044019937515259) {
                    if (input[2] <= 3.6054816842079163) {
                        var7 = new double[3] {0.0, 0.0, 1.0};
                    } else {
                        var7 = new double[3] {0.0, 1.0, 0.0};
                    }
                } else {
                    var7 = new double[3] {1.0, 0.0, 0.0};
                }
            }
            double[] var8;
            if (input[2] <= 6.3480064868927) {
                if (input[1] <= 1.0563380420207977) {
                    if (input[0] <= 1.5370370149612427) {
                        if (input[0] <= 0.9541062712669373) {
                            var8 = new double[3] {0.0, 0.2, 0.8};
                        } else {
                            var8 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        if (input[1] <= 0.47887323796749115) {
                            var8 = new double[3] {1.0, 0.0, 0.0};
                        } else {
                            var8 = new double[3] {0.45161290322580644, 0.0, 0.5483870967741935};
                        }
                    }
                } else {
                    var8 = new double[3] {0.0, 1.0, 0.0};
                }
            } else {
                var8 = new double[3] {1.0, 0.0, 0.0};
            }
            double[] var9;
            if (input[0] <= 5.919484853744507) {
                if (input[1] <= 1.0563380420207977) {
                    if (input[0] <= 1.5249597430229187) {
                        if (input[0] <= 0.9597423374652863) {
                            var9 = new double[3] {0.0, 0.3333333333333333, 0.6666666666666666};
                        } else {
                            var9 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        if (input[2] <= 1.745847225189209) {
                            var9 = new double[3] {0.75, 0.0, 0.25};
                        } else {
                            var9 = new double[3] {0.3448275862068966, 0.0, 0.6551724137931034};
                        }
                    }
                } else {
                    var9 = new double[3] {0.0, 1.0, 0.0};
                }
            } else {
                if (input[0] <= 8.610305786132812) {
                    var9 = new double[3] {1.0, 0.0, 0.0};
                } else {
                    if (input[0] <= 8.88164234161377) {
                        var9 = new double[3] {0.0, 0.0, 1.0};
                    } else {
                        var9 = new double[3] {1.0, 0.0, 0.0};
                    }
                }
            }
            double[] var10;
            if (input[0] <= 6.020934104919434) {
                if (input[2] <= 1.419435203075409) {
                    if (input[0] <= 1.3373591303825378) {
                        if (input[2] <= 1.1719269156455994) {
                            var10 = new double[3] {0.0, 0.3380281690140845, 0.6619718309859155};
                        } else {
                            var10 = new double[3] {0.0, 0.5116279069767442, 0.4883720930232558};
                        }
                    } else {
                        var10 = new double[3] {0.0, 0.0, 1.0};
                    }
                } else {
                    if (input[2] <= 1.8936877250671387) {
                        if (input[1] <= 2.366197168827057) {
                            var10 = new double[3] {0.8620689655172413, 0.0, 0.13793103448275862};
                        } else {
                            var10 = new double[3] {0.0, 1.0, 0.0};
                        }
                    } else {
                        if (input[1] <= 1.99295774102211) {
                            var10 = new double[3] {0.21428571428571427, 0.0, 0.7857142857142857};
                        } else {
                            var10 = new double[3] {0.0, 1.0, 0.0};
                        }
                    }
                }
            } else {
                if (input[2] <= 6.044019937515259) {
                    if (input[0] <= 14.769726276397705) {
                        var10 = new double[3] {0.0, 0.0, 1.0};
                    } else {
                        var10 = new double[3] {0.0, 1.0, 0.0};
                    }
                } else {
                    var10 = new double[3] {1.0, 0.0, 0.0};
                }
            }
            double[] var11;
            if (input[2] <= 6.275747537612915) {
                if (input[1] <= 1.0563380420207977) {
                    if (input[0] <= 1.4847021102905273) {
                        if (input[1] <= 0.6126760542392731) {
                            var11 = new double[3] {0.0, 0.0392156862745098, 0.9607843137254902};
                        } else {
                            var11 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        if (input[2] <= 4.388704299926758) {
                            var11 = new double[3] {0.6829268292682927, 0.0, 0.3170731707317073};
                        } else {
                            var11 = new double[3] {0.0, 1.0, 0.0};
                        }
                    }
                } else {
                    var11 = new double[3] {0.0, 1.0, 0.0};
                }
            } else {
                if (input[2] <= 16.260797023773193) {
                    var11 = new double[3] {1.0, 0.0, 0.0};
                } else {
                    if (input[2] <= 23.519103050231934) {
                        var11 = new double[3] {0.0, 1.0, 0.0};
                    } else {
                        var11 = new double[3] {1.0, 0.0, 0.0};
                    }
                }
            }
            double[] var12;
            if (input[2] <= 6.275747537612915) {
                if (input[0] <= 1.6658614873886108) {
                    if (input[0] <= 1.2246376872062683) {
                        if (input[1] <= 1.0563380420207977) {
                            var12 = new double[3] {0.0, 0.016666666666666666, 0.9833333333333333};
                        } else {
                            var12 = new double[3] {0.0, 1.0, 0.0};
                        }
                    } else {
                        if (input[1] <= 1.4436619579792023) {
                            var12 = new double[3] {0.14285714285714285, 0.0, 0.8571428571428571};
                        } else {
                            var12 = new double[3] {0.0, 1.0, 0.0};
                        }
                    }
                } else {
                    if (input[1] <= 2.0352112352848053) {
                        if (input[1] <= 0.7676056325435638) {
                            var12 = new double[3] {0.4444444444444444, 0.05555555555555555, 0.5};
                        } else {
                            var12 = new double[3] {1.0, 0.0, 0.0};
                        }
                    } else {
                        var12 = new double[3] {0.0, 1.0, 0.0};
                    }
                }
            } else {
                var12 = new double[3] {1.0, 0.0, 0.0};
            }
            double[] var13;
            if (input[2] <= 6.196843862533569) {
                if (input[0] <= 1.2455716729164124) {
                    if (input[1] <= 1.0563380420207977) {
                        if (input[0] <= 0.9533011317253113) {
                            var13 = new double[3] {0.0, 0.25, 0.75};
                        } else {
                            var13 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        var13 = new double[3] {0.0, 1.0, 0.0};
                    }
                } else {
                    if (input[1] <= 2.088028162717819) {
                        if (input[2] <= 1.540697693824768) {
                            var13 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var13 = new double[3] {0.5581395348837209, 0.0, 0.4418604651162791};
                        }
                    } else {
                        var13 = new double[3] {0.0, 1.0, 0.0};
                    }
                }
            } else {
                if (input[0] <= 109.9975814819336) {
                    var13 = new double[3] {1.0, 0.0, 0.0};
                } else {
                    var13 = new double[3] {0.0, 1.0, 0.0};
                }
            }
            double[] var14;
            if (input[1] <= 1.0563380420207977) {
                if (input[1] <= 0.7676056325435638) {
                    if (input[0] <= 1.4959742426872253) {
                        var14 = new double[3] {0.0, 0.0, 1.0};
                    } else {
                        if (input[1] <= 0.47535212337970734) {
                            var14 = new double[3] {1.0, 0.0, 0.0};
                        } else {
                            var14 = new double[3] {0.5428571428571428, 0.02857142857142857, 0.42857142857142855};
                        }
                    }
                } else {
                    if (input[1] <= 0.9436619877815247) {
                        if (input[2] <= 2.120431900024414) {
                            var14 = new double[3] {0.26666666666666666, 0.0, 0.7333333333333333};
                        } else {
                            var14 = new double[3] {1.0, 0.0, 0.0};
                        }
                    } else {
                        var14 = new double[3] {0.0, 0.0, 1.0};
                    }
                }
            } else {
                var14 = new double[3] {0.0, 1.0, 0.0};
            }
            double[] var15;
            if (input[1] <= 1.0563380420207977) {
                if (input[2] <= 1.540697693824768) {
                    if (input[0] <= 0.9533011317253113) {
                        if (input[2] <= 1.02491694688797) {
                            var15 = new double[3] {0.0, 0.5, 0.5};
                        } else {
                            var15 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        var15 = new double[3] {0.0, 0.0, 1.0};
                    }
                } else {
                    if (input[0] <= 5.03542685508728) {
                        if (input[2] <= 1.9277408719062805) {
                            var15 = new double[3] {0.8148148148148148, 0.0, 0.18518518518518517};
                        } else {
                            var15 = new double[3] {0.375, 0.0, 0.625};
                        }
                    } else {
                        if (input[2] <= 4.361295759677887) {
                            var15 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var15 = new double[3] {1.0, 0.0, 0.0};
                        }
                    }
                }
            } else {
                var15 = new double[3] {0.0, 1.0, 0.0};
            }
            double[] var16;
            if (input[0] <= 6.026570081710815) {
                if (input[2] <= 1.4418604373931885) {
                    if (input[2] <= 0.998338907957077) {
                        if (input[2] <= 0.9543189406394958) {
                            var16 = new double[3] {0.0, 0.2, 0.8};
                        } else {
                            var16 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        if (input[1] <= 1.0563380420207977) {
                            var16 = new double[3] {0.0, 0.011494252873563218, 0.9885057471264368};
                        } else {
                            var16 = new double[3] {0.0, 1.0, 0.0};
                        }
                    }
                } else {
                    if (input[2] <= 2.745847225189209) {
                        if (input[1] <= 2.133802831172943) {
                            var16 = new double[3] {0.6285714285714286, 0.0, 0.37142857142857144};
                        } else {
                            var16 = new double[3] {0.0, 1.0, 0.0};
                        }
                    } else {
                        if (input[1] <= 30.908451467752457) {
                            var16 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var16 = new double[3] {0.0, 1.0, 0.0};
                        }
                    }
                }
            } else {
                if (input[0] <= 109.9975814819336) {
                    var16 = new double[3] {1.0, 0.0, 0.0};
                } else {
                    var16 = new double[3] {0.0, 1.0, 0.0};
                }
            }
            double[] var17;
            if (input[0] <= 6.020934104919434) {
                if (input[1] <= 1.0563380420207977) {
                    if (input[1] <= 0.47535212337970734) {
                        if (input[0] <= 1.3888888955116272) {
                            var17 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var17 = new double[3] {1.0, 0.0, 0.0};
                        }
                    } else {
                        if (input[2] <= 1.614617943763733) {
                            var17 = new double[3] {0.0, 0.0379746835443038, 0.9620253164556962};
                        } else {
                            var17 = new double[3] {0.4444444444444444, 0.0, 0.5555555555555556};
                        }
                    }
                } else {
                    var17 = new double[3] {0.0, 1.0, 0.0};
                }
            } else {
                if (input[2] <= 6.044019937515259) {
                    if (input[0] <= 14.769726276397705) {
                        var17 = new double[3] {0.0, 0.0, 1.0};
                    } else {
                        var17 = new double[3] {0.0, 1.0, 0.0};
                    }
                } else {
                    var17 = new double[3] {1.0, 0.0, 0.0};
                }
            }
            double[] var18;
            if (input[1] <= 1.0563380420207977) {
                if (input[0] <= 1.6312399506568909) {
                    if (input[0] <= 0.9533011317253113) {
                        if (input[0] <= 0.9460547566413879) {
                            var18 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var18 = new double[3] {0.0, 1.0, 0.0};
                        }
                    } else {
                        if (input[2] <= 1.725913643836975) {
                            var18 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var18 = new double[3] {0.3333333333333333, 0.0, 0.6666666666666666};
                        }
                    }
                } else {
                    if (input[2] <= 2.265780806541443) {
                        if (input[2] <= 1.7516611218452454) {
                            var18 = new double[3] {1.0, 0.0, 0.0};
                        } else {
                            var18 = new double[3] {0.35294117647058826, 0.0, 0.6470588235294118};
                        }
                    } else {
                        if (input[1] <= 0.7605633735656738) {
                            var18 = new double[3] {0.9285714285714286, 0.07142857142857142, 0.0};
                        } else {
                            var18 = new double[3] {1.0, 0.0, 0.0};
                        }
                    }
                }
            } else {
                var18 = new double[3] {0.0, 1.0, 0.0};
            }
            double[] var19;
            if (input[2] <= 6.292358875274658) {
                if (input[2] <= 1.4410299062728882) {
                    if (input[1] <= 1.4049296081066132) {
                        if (input[0] <= 0.9533011317253113) {
                            var19 = new double[3] {0.0, 0.25, 0.75};
                        } else {
                            var19 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        var19 = new double[3] {0.0, 1.0, 0.0};
                    }
                } else {
                    if (input[1] <= 2.133802831172943) {
                        if (input[2] <= 1.5714285969734192) {
                            var19 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var19 = new double[3] {0.6341463414634146, 0.024390243902439025, 0.34146341463414637};
                        }
                    } else {
                        var19 = new double[3] {0.0, 1.0, 0.0};
                    }
                }
            } else {
                var19 = new double[3] {1.0, 0.0, 0.0};
            }
            double[] var20;
            if (input[1] <= 1.0563380420207977) {
                if (input[2] <= 1.6063122749328613) {
                    if (input[2] <= 1.0215947031974792) {
                        if (input[0] <= 0.9597423374652863) {
                            var20 = new double[3] {0.0, 0.5, 0.5};
                        } else {
                            var20 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        var20 = new double[3] {0.0, 0.0, 1.0};
                    }
                } else {
                    if (input[0] <= 4.921900272369385) {
                        if (input[2] <= 1.9368770718574524) {
                            var20 = new double[3] {0.8333333333333334, 0.0, 0.16666666666666666};
                        } else {
                            var20 = new double[3] {0.29411764705882354, 0.0, 0.7058823529411765};
                        }
                    } else {
                        if (input[2] <= 6.044019937515259) {
                            var20 = new double[3] {0.0, 0.5, 0.5};
                        } else {
                            var20 = new double[3] {1.0, 0.0, 0.0};
                        }
                    }
                }
            } else {
                var20 = new double[3] {0.0, 1.0, 0.0};
            }
            double[] var21;
            if (input[0] <= 6.137681245803833) {
                if (input[0] <= 1.3913043141365051) {
                    if (input[0] <= 1.0466988682746887) {
                        if (input[1] <= 1.0563380420207977) {
                            var21 = new double[3] {0.0, 0.125, 0.875};
                        } else {
                            var21 = new double[3] {0.0, 1.0, 0.0};
                        }
                    } else {
                        if (input[2] <= 1.2192690968513489) {
                            var21 = new double[3] {0.0, 0.1276595744680851, 0.8723404255319149};
                        } else {
                            var21 = new double[3] {0.0, 0.34146341463414637, 0.6585365853658537};
                        }
                    }
                } else {
                    if (input[0] <= 2.6352657079696655) {
                        if (input[2] <= 1.7234219312667847) {
                            var21 = new double[3] {0.15, 0.525, 0.325};
                        } else {
                            var21 = new double[3] {0.42857142857142855, 0.39285714285714285, 0.17857142857142858};
                        }
                    } else {
                        var21 = new double[3] {0.0, 1.0, 0.0};
                    }
                }
            } else {
                if (input[2] <= 4.27574759721756) {
                    var21 = new double[3] {0.0, 0.0, 1.0};
                } else {
                    if (input[2] <= 23.416943550109863) {
                        if (input[2] <= 16.260797023773193) {
                            var21 = new double[3] {1.0, 0.0, 0.0};
                        } else {
                            var21 = new double[3] {0.0, 1.0, 0.0};
                        }
                    } else {
                        var21 = new double[3] {1.0, 0.0, 0.0};
                    }
                }
            }
            double[] var22;
            if (input[1] <= 1.4049296081066132) {
                if (input[2] <= 1.5714285969734192) {
                    if (input[2] <= 1.0215947031974792) {
                        if (input[2] <= 1.0132890343666077) {
                            var22 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var22 = new double[3] {0.0, 1.0, 0.0};
                        }
                    } else {
                        var22 = new double[3] {0.0, 0.0, 1.0};
                    }
                } else {
                    if (input[1] <= 0.4647887349128723) {
                        if (input[1] <= 0.42957746982574463) {
                            var22 = new double[3] {0.9615384615384616, 0.0, 0.038461538461538464};
                        } else {
                            var22 = new double[3] {1.0, 0.0, 0.0};
                        }
                    } else {
                        if (input[1] <= 0.7183098495006561) {
                            var22 = new double[3] {0.4482758620689655, 0.034482758620689655, 0.5172413793103449};
                        } else {
                            var22 = new double[3] {0.8846153846153846, 0.019230769230769232, 0.09615384615384616};
                        }
                    }
                }
            } else {
                var22 = new double[3] {0.0, 1.0, 0.0};
            }
            double[] var23;
            if (input[1] <= 1.0563380420207977) {
                if (input[2] <= 1.540697693824768) {
                    if (input[2] <= 1.0215947031974792) {
                        if (input[0] <= 0.9533011317253113) {
                            var23 = new double[3] {0.0, 0.4, 0.6};
                        } else {
                            var23 = new double[3] {0.0, 0.0, 1.0};
                        }
                    } else {
                        var23 = new double[3] {0.0, 0.0, 1.0};
                    }
                } else {
                    if (input[2] <= 6.044019937515259) {
                        if (input[0] <= 2.0330111980438232) {
                            var23 = new double[3] {0.7631578947368421, 0.0, 0.23684210526315788};
                        } else {
                            var23 = new double[3] {0.21052631578947367, 0.05263157894736842, 0.7368421052631579};
                        }
                    } else {
                        if (input[2] <= 23.43853759765625) {
                            var23 = new double[3] {0.9375, 0.0625, 0.0};
                        } else {
                            var23 = new double[3] {1.0, 0.0, 0.0};
                        }
                    }
                }
            } else {
                var23 = new double[3] {0.0, 1.0, 0.0};
            }
            double[] var24;
            if (input[1] <= 1.4049296081066132) {
                if (input[2] <= 1.500830590724945) {
                    if (input[0] <= 0.9533011317253113) {
                        if (input[0] <= 0.9460547566413879) {
                            var24 = new double[3] {0.0, 0.0, 1.0};
                        } else {
                            var24 = new double[3] {0.0, 1.0, 0.0};
                        }
                    } else {
                        var24 = new double[3] {0.0, 0.0, 1.0};
                    }
                } else {
                    if (input[0] <= 4.921900272369385) {
                        if (input[1] <= 0.4999999850988388) {
                            var24 = new double[3] {1.0, 0.0, 0.0};
                        } else {
                            var24 = new double[3] {0.391304347826087, 0.0, 0.6086956521739131};
                        }
                    } else {
                        if (input[2] <= 6.044019937515259) {
                            var24 = new double[3] {0.0, 0.3333333333333333, 0.6666666666666666};
                        } else {
                            var24 = new double[3] {1.0, 0.0, 0.0};
                        }
                    }
                }
            } else {
                var24 = new double[3] {0.0, 1.0, 0.0};
            }
            return MulVectorNumber(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(AddVectors(var0, var1), var2), var3), var4), var5), var6), var7), var8), var9), var10), var11), var12), var13), var14), var15), var16), var17), var18), var19), var20), var21), var22), var23), var24), 0.04);
        }
        private static double[] AddVectors(double[] v1, double[] v2) {
            double[] result = new double[v1.Length];
            for (int i = 0; i < v1.Length; ++i) {
                result[i] = v1[i] + v2[i];
            }
            return result;
        }
        private static double[] MulVectorNumber(double[] v1, double num) {
            double[] result = new double[v1.Length];
            for (int i = 0; i < v1.Length; ++i) {
                result[i] = v1[i] * num;
            }
            return result;
        }
    }
}
