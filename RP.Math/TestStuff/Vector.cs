#region Imports

using System;
using System.Xml.Serialization;			// for various Xml attributes

#endregion

namespace RPUtil.Math.Math3D
{
    /// <summary>
    /// vector of doubles with three components (_x,_y,_z)
    /// </summary>
    /// <author>Richard Potter BSc(Hons)</author>
    /// <created>Jun-04</created>
    /// <modified>Jul-08</modified>
    /// <version>1.22</version>
    /// <Changes>
    /// Magnitude is now a property
    /// Abs(...) now returns magnitude, Recommend: use magnitude property instead
    /// Equality opeartions now have a tolerance (note that greater and less than type operations do not)
    /// IsUnit methods also have a tolerence
    /// Generic IEquatable and IComparable interfaces implemented
    /// IFormattable interface (ToString(format, format provider) implemented
    /// Mixed product function implemented
    /// SqrComponents was calling square root function
    /// Several functions took a parameter 'degree' but was actualy using the value as radian
    /// ToString format string now case insensitive
    /// ToVerbString method now only available as 'v' format string character ( ToString(format,provider) )
    /// Added array accessor using _x,_y,_z characters in addition to integers 1,2,3
    /// Renamed to Vector
    /// Axial system awareness implemented
    /// </Changes>
    /// <Note>
    /// MAY-07 
    /// Tried to convert to a generic Vector Type. 
    /// This is way more complicated than it seems as there seems to be no numeric type interface. 
    /// Result of this is that you cannot use any mathematical operators and you have to encode runtime type checking.
    /// Factory would be a better pattern but is beyond scope\requirements.
    /// JULY-07
    /// Made Immutable (static tolerance is mutable)
    /// DEC-07
    /// Static tolerance has been removed - it feels like overkill and likely to cause unexpected problems for the developer
    /// Replaced with equality functions which take tolerance as a parameter - Operators use .Net double default tolerances
    /// Added Rounding methods
    /// JUL-08
    /// Fixed multiple axis mapping bugs
    /// Added a vector and magnitude string format character (m)
    /// </Note>
    [Serializable]
    public struct Vector
        : IComparable, IComparable<Vector>, IEquatable<Vector>, IFormattable
    {

        #region Struct Variables

        /// <summary>
        /// The X component of the vector
        /// </summary>
        private readonly double _x;

        /// <summary>
        /// The Y component of the vector
        /// </summary>
        private readonly double _y;

        /// <summary>
        /// The Z component of the vector
        /// </summary>
        private readonly double _z;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the Vector class accepting three doubles
        /// </summary>
        /// <param name="x">The new _x value for the Vector</param>
        /// <param name="y">The new _y value for the Vector</param>
        /// <param name="z">The new _z value for the Vector</param>
        public Vector(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Constructor for the Vector class from an array
        /// </summary>
        /// <param name="xyz">Array representing the new values for the Vector</param>
        public Vector(double[] xyz)
        {
            if (xyz.Length != 3) throw new ArgumentException(THREE_COMPONENTS, "xyz");
            _x = xyz[0];
            _y = xyz[1];
            _z = xyz[2];
        }

        /// <summary>
        /// Constructor for the Vector class from another Vector object
        /// </summary>
        /// <param name="v1">Vector representing the new values for the Vector</param>
        /// <implementation>
        /// Copies values from Vector v1 to this vector, does not hold a reference to object v1 
        /// </implementation>
        public Vector(Vector v1)
        {
            _x = v1.X;
            _y = v1.Y;
            _z = v1.Z;
        }

        #endregion

        #region Accessors & Mutators

        /// <summary>
        /// Length for the _x component of the Vector
        /// </summary>
        public double X
        {
            get { return _x; }
        }

        /// <summary>
        /// Length for the _y component of the Vector
        /// </summary>
        public double Y
        {
            get { return _y; }
        }

        /// <summary>
        /// Length for the _z component of the Vector
        /// </summary>
        public double Z
        {
            get { return _z; }
        }

        /// <summary>
        /// Length for the magnitude (aka. length or absolute value) of the Vector
        /// </summary>
        public double Magnitude
        {
            get
            {
                return
                System.Math.Sqrt(SumComponentSqrs());
            }
        }

        /// <summary>
        /// Length for the Vector as an array
        /// </summary>
        [XmlIgnore]
        public double[] Array
        {
            get { return new[] { _x, _y, _z }; }
        }

        /// <summary>
        /// An index accessor 
        /// Mapping index [0] -> X, [1] -> Y and [2] -> Z.
        /// </summary>
        /// <param name="index">The array index referring to a component within the vector (i.e. _x, _y, _z)</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the array argument does not contain exactly three components 
        /// </exception>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: { return X; }
                    case 1: { return Y; }
                    case 2: { return Z; }
                    default: throw new ArgumentOutOfRangeException("index", index, THREE_COMPONENTS);
                }
            }
        }

        /// <summary>
        /// An index accessor 
        /// Mapping index [0] -> X, [1] -> Y and [2] -> Z.
        /// </summary>
        /// <param name="index">The array index character referring to a component within the vector (i.e. _x, _y, _z)</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the index is not between _x-_z
        /// </exception>
        public double this[char index]
        {
            get
            {
                switch (index)
                {
                    case 'x': { return X; }
                    case 'y': { return Y; }
                    case 'z': { return Z; }
                    default: throw new ArgumentException(THREE_COMPONENTS, "index");
                }
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Addition of two Vectors
        /// </summary>
        /// <param name="v1">Vector to be added to </param>
        /// <param name="v2">Vector to be added</param>
        /// <returns>Vector representing the sum of two Vectors</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static Vector operator +(Vector v1, Vector v2)
        {
            return
            (
                new Vector
                    (
                        v1.X + v2.X,
                        v1.Y + v2.Y,
                        v1.Z + v2.Z
                    )
            );
        }

        /// <summary>
        /// Subtraction of two Vectors
        /// </summary>
        /// <param name="v1">Vector to be subtracted from </param>
        /// <param name="v2">Vector to be subtracted</param>
        /// <returns>Vector representing the difference of two Vectors</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static Vector operator -(Vector v1, Vector v2)
        {
            return
            (
                new Vector
                    (
                        v1.X - v2.X,
                        v1.Y - v2.Y,
                        v1.Z - v2.Z
                    )
            );
        }

        /// <summary>
        /// Product of a Vector and a scalar value
        /// </summary>
        /// <param name="v1">Vector to be multiplied </param>
        /// <param name="s2">Scalar value to be multiplied by </param>
        /// <returns>Vector representing the product of the vector and scalar</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static Vector operator *(Vector v1, double s2)
        {
            return
            (
                new Vector
                (
                    v1.X * s2,
                    v1.Y * s2,
                    v1.Z * s2
                )
            );
        }

        /// <summary>
        /// Product of a scalar value and a Vector
        /// </summary>
        /// <param name="s1">Scalar value to be multiplied </param>
        /// <param name="v2">Vector to be multiplied by </param>
        /// <returns>Vector representing the product of the scalar and Vector</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        /// <Implementation>
        /// Using the commutative law 'scalar _x vector'='vector _x scalar'.
        /// Thus, this function calls 'operator*(Vector v1, double s2)'.
        /// This avoids repetition of code.
        /// </Implementation>
        public static Vector operator *(double s1, Vector v2)
        {
            return v2 * s1;
        }

        /// <summary>
        /// Division of a Vector and a scalar value
        /// </summary>
        /// <param name="v1">Vector to be divided </param>
        /// <param name="s2">Scalar value to be divided by </param>
        /// <returns>Vector representing the division of the vector and scalar</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static Vector operator /(Vector v1, double s2)
        {
            return
            (
                new Vector
                    (
                        v1.X / s2,
                        v1.Y / s2,
                        v1.Z / s2
                    )
            );
        }

        /// <summary>
        /// Negation of a Vector
        /// Invert the direction of the Vector
        /// Make Vector negative (-vector)
        /// </summary>
        /// <param name="v1">Vector to be negated  </param>
        /// <returns>Negated vector</returns>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static Vector operator -(Vector v1)
        {
            return
            (
                new Vector
                    (
                        -v1.X,
                        -v1.Y,
                        -v1.Z
                    )
            );
        }

        /// <summary>
        /// Reinforcement of a Vector
        /// Make Vector positive (+vector)
        /// </summary>
        /// <param name="v1">Vector to be reinforced </param>
        /// <returns>Reinforced vector</returns>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        /// <Implementation>
        /// Using the rules of Addition (i.e. '+-_x' = '-_x' and '++_x' = '+_x')
        /// This function actually does nothing but return the argument as given
        /// </Implementation>
        public static Vector operator +(Vector v1)
        {
            return
            (
                new Vector
                    (
                        +v1.X,
                        +v1.Y,
                        +v1.Z
                    )
            );
        }

        /// <summary>
        /// Compare the magnitude of two Vectors (less than)
        /// </summary>
        /// <param name="v1">Vector to be compared </param>
        /// <param name="v2">Vector to be compared with</param>
        /// <returns>True if v1 less than v2</returns>
        public static bool operator <(Vector v1, Vector v2)
        {
            return v1.SumComponentSqrs() < v2.SumComponentSqrs();
        }

        /// <summary>
        /// Compare the magnitude of two Vectors (greater than)
        /// </summary>
        /// <param name="v1">Vector to be compared </param>
        /// <param name="v2">Vector to be compared with</param>
        /// <returns>True if v1 greater than v2</returns>
        public static bool operator >(Vector v1, Vector v2)
        {
            return v1.SumComponentSqrs() > v2.SumComponentSqrs();
        }

        /// <summary>
        /// Compare the magnitude of two Vectors (less than or equal to)
        /// </summary>
        /// <param name="v1">Vector to be compared </param>
        /// <param name="v2">Vector to be compared with</param>
        /// <returns>True if v1 less than or equal to v2</returns>
        public static bool operator <=(Vector v1, Vector v2)
        {
            return v1.SumComponentSqrs() <= v2.SumComponentSqrs();
        }

        /// <summary>
        /// Compare the magnitude of two Vectors (greater than or equal to)
        /// </summary>
        /// <param name="v1">Vector to be compared </param>
        /// <param name="v2">Vector to be compared with</param>
        /// <returns>True if v1 greater than or equal to v2</returns>
        public static bool operator >=(Vector v1, Vector v2)
        {
            return v1.SumComponentSqrs() >= v2.SumComponentSqrs();
        }

        /// <summary>
        /// Compare two Vectors for equality.
        /// Are two Vectors equal.
        /// </summary>
        /// <param name="v1">Vector to be compared for equality </param>
        /// <param name="v2">Vector to be compared to </param>
        /// <returns>Boolean decision (truth for equality)</returns>
        /// <implementation>
        /// Checks the equality of each pair of components, all pairs must be equal
        /// </implementation>
        public static bool operator ==(Vector v1, Vector v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }

        /// <summary>
        /// Negative comparator of two Vectors.
        /// Are two Vectors different.
        /// </summary>
        /// <param name="v1">Vector to be compared for in-equality </param>
        /// <param name="v2">Vector to be compared to </param>
        /// <returns>Boolean decision (truth for in-equality)</returns>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        /// <implementation>
        /// Uses the equality operand function for two vectors to prevent code duplication
        /// </implementation>
        public static bool operator !=(Vector v1, Vector v2)
        {
            return !(v1 == v2);
        }

        #endregion

        #region Functions

        /// <summary>
        /// Determine the cross product of two Vectors
        /// Determine the vector product
        /// Determine the normal vector (Vector 90° to the plane)
        /// </summary>
        /// <param name="v1">The vector to multiply</param>
        /// <param name="v2">The vector to multiply by</param>
        /// <returns>Vector representing the cross product of the two vectors</returns>
        /// <implementation>
        /// Cross products are non commutable
        /// </implementation>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static Vector CrossProduct(Vector v1, Vector v2)
        {
            return
            (
                new Vector
                (
                    v1.Y * v2.Z - v1.Z * v2.Y,
                    v1.Z * v2.X - v1.X * v2.Z,
                    v1.X * v2.Y - v1.Y * v2.X
                )
            );
        }

        /// <summary>
        /// Determine the cross product of this Vector and another
        /// Determine the vector product
        /// Determine the normal vector (Vector 90° to the plane)
        /// </summary>
        /// <param name="other">The vector to multiply by</param>
        /// <returns>Vector representing the cross product of the two vectors</returns>
        /// <implementation>
        /// Uses the CrossProduct function to avoid code duplication
        /// <see cref="CrossProduct(Vector, Vector)"/>
        /// </implementation>
        public Vector CrossProduct(Vector other)
        {
            return CrossProduct(this, other);
        }

        /// <summary>
        /// Determine the dot product of two Vectors
        /// </summary>
        /// <param name="v1">The vector to multiply</param>
        /// <param name="v2">The vector to multiply by</param>
        /// <returns>Scalar representing the dot product of the two vectors</returns>
        /// <implementation>
        /// </implementation>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public static double DotProduct(Vector v1, Vector v2)
        {
            return
            (
                v1.X * v2.X +
                v1.Y * v2.Y +
                v1.Z * v2.Z
            );
        }

        /// <summary>
        /// Determine the dot product of this Vector and another
        /// </summary>
        /// <param name="other">The vector to multiply by</param>
        /// <returns>Scalar representing the dot product of the two vectors</returns>
        /// <implementation>
        /// <see cref="DotProduct(Vector)"/>
        /// </implementation>
        public double DotProduct(Vector other)
        {
            return DotProduct(this, other);
        }

        /// <summary>
        /// Determine the mixed product of three Vectors
        /// Determine volume (with sign precision) of parallelepiped spanned on given vectors
        /// Determine the scalar triple product of three vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="v3">The third vector</param>
        /// <returns>Scalar representing the mixed product of the three vectors</returns>
        /// <implementation>
        /// Mixed products are non commutable
        /// <see cref="CrossProduct(Vector, Vector)"/>
        /// <see cref="DotProduct(Vector, Vector)"/>
        /// </implementation>
        /// <Acknowledgement>This code was provided by Michał Bryłka</Acknowledgement>
        public static double MixedProduct(Vector v1, Vector v2, Vector v3)
        {
            return DotProduct(CrossProduct(v1, v2), v3);
        }

        /// <summary>
        /// Determine the mixed product of three Vectors
        /// Determine volume (with sign precision) of parallelepiped spanned on given vectors
        /// Determine the scalar triple product of three vectors
        /// </summary>
        /// <param name="other_v1">The second vector</param>
        /// <param name="other_v2">The third vector</param>
        /// <returns>Scalar representing the mixed product of the three vectors</returns>
        /// <implementation>
        /// Mixed products are non commutable
        /// <see cref="MixedProduct(Vector, Vector, Vector)"/>
        /// Uses MixedProduct(Vector, Vector, Vector) to avoid code duplication
        /// </implementation>
        public double MixedProduct(Vector other_v1, Vector other_v2)
        {
            return DotProduct(CrossProduct(this, other_v1), other_v2);
        }

        /// <summary>
        /// Get the normalized vector
        /// Get the unit vector
        /// Scale the Vector so that the magnitude is 1
        /// </summary>
        /// <param name="v1">The vector to be normalized</param>
        /// <returns>The normalized Vector</returns>
        /// <implementation>
        /// Uses the Magnitude function to avoid code duplication 
        /// </implementation>
        /// <exception cref="System.DivideByZeroException">
        /// Thrown when the normalisation of a zero magnitude vector is attempted
        /// </exception>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static Vector Normalize(Vector v1)
        {
            // Check for divide by zero errors
            if (v1.Magnitude == 0)
                throw new DivideByZeroException(NORMALIZE_0);
            
            // find the inverse of the vectors magnitude
            double inverse = 1 / v1.Magnitude;
            return
                (
                    new Vector
                        (
                        // multiply each component by the inverse of the magnitude
                        v1.X * inverse,
                        v1.Y * inverse,
                        v1.Z * inverse
                        )
                );
        }

        /// <summary>
        /// Get the normalized vector
        /// Get the unit vector
        /// Scale the Vector so that the magnitude is 1
        /// </summary>
        /// <returns>The normalized Vector</returns>
        /// <implementation>
        /// Uses the Magnitude and Normalize function to avoid code duplication 
        /// </implementation>
        /// <exception cref="System.DivideByZeroException">
        /// Thrown when the normalisation of a zero magnitude vector is attempted
        /// </exception>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public Vector Normalize()
        {
            return Normalize(this);
        }

        /// <summary>
        /// Take an interpolated value from between two Vectors or an extrapolated value if allowed
        /// </summary>
        /// <param name="v1">The Vector to interpolate from (where control ==0)</param>
        /// <param name="v2">The Vector to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1), or an extrapolated point if allowed</param>
        /// <param name="allowExtrapolation">True if the control may represent a point not on the vertex between v1 and v2</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors or an extrapolated point on the extended virtex</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the control is not between values of 0 and 1 and extrapolation is not allowed
        /// </exception>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static Vector Interpolate(Vector v1, Vector v2, double control, bool allowExtrapolation)
        {
            if (!allowExtrapolation && (control > 1 || control < 0))
                // Error message includes information about the actual value of the argument
                throw new ArgumentOutOfRangeException
                        (
                            "control",
                            control,
                            INTERPOLATION_RANGE + "\n" + ARGUMENT_VALUE + control
                        );
            
            return
                (
                    new Vector
                        (
                        v1.X * (1 - control) + v2.X * control,
                        v1.Y * (1 - control) + v2.Y * control,
                        v1.Z * (1 - control) + v2.Z * control
                        )
                );
        }

        /// <summary>
        /// Take an interpolated value from between two Vectors
        /// </summary>
        /// <param name="v1">The Vector to interpolate from (where control ==0)</param>
        /// <param name="v2">The Vector to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1)</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors</returns>
        /// <implementation>
        /// <see cref="Interpolate(Vector, Vector, double, bool)"/>
        /// Uses the Interpolate(Vector,Vector,double,bool) method to avoid code duplication
        /// </implementation>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the control is not between values of 0 and 1
        /// </exception>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static Vector Interpolate(Vector v1, Vector v2, double control)
        {
            return Interpolate(v1, v2, control, false);
        }


        /// <summary>
        /// Take an interpolated value from between two Vectors
        /// </summary>
        /// <param name="other">The Vector to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1)</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors</returns>
        /// <implementation>
        /// <see cref="Interpolate(Vector, Vector, double)"/>
        /// Overload for Interpolate method, finds an interpolated value between this Vector and another
        /// Uses the Interpolate(Vector,Vector,double) method to avoid code duplication
        /// </implementation>
        public Vector Interpolate(Vector other, double control)
        {
            return Interpolate(this, other, control);
        }

        /// <summary>
        /// Take an interpolated value from between two Vectors or an extrapolated value if allowed
        /// </summary>
        /// <param name="other">The Vector to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1), or an extrapolated point if allowed</param>
        /// <param name="allowExtrapolation">True if the control may represent a point not on the vertex between v1 and v2</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors or an extrapolated point on the extended virtex</returns>
        /// <implementation>
        /// <see cref="Interpolate(Vector, Vector, double, bool)"/>
        /// Uses the Interpolate(Vector,Vector,double,bool) method to avoid code duplication
        /// </implementation>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the control is not between values of 0 and 1 and extrapolation is not allowed
        /// </exception>
        public Vector Interpolate(Vector other, double control, bool allowExtrapolation)
        {
            return Interpolate(this, other, control);
        }

        /// <summary>
        /// FindPlugins the distance between two Vectors
        /// Pythagoras theorem on two Vectors
        /// </summary>
        /// <param name="v1">The Vector to find the distance from </param>
        /// <param name="v2">The Vector to find the distance to </param>
        /// <returns>The distance between two Vectors</returns>
        /// <implementation>
        /// </implementation>
        public static double Distance(Vector v1, Vector v2)
        {
            return
            (
                System.Math.Sqrt
                (
                    (v1.X - v2.X) * (v1.X - v2.X) +
                    (v1.Y - v2.Y) * (v1.Y - v2.Y) +
                    (v1.Z - v2.Z) * (v1.Z - v2.Z)
                )
            );
        }

        /// <summary>
        /// Finds the distance between the heads of two Vectors
        /// Pythagoras theorem on two Vectors
        /// </summary>
        /// <param name="other">The Vector to find the distance to </param>
        /// <returns>The distance between two Vectors</returns>
        /// <implementation>
        /// <see cref="Distance(Vector, Vector)"/>
        /// Overload for Distance method, finds distance between this Vector and another
        /// Uses the Distance(Vector,Vector) method to avoid code duplication
        /// </implementation>
        public double Distance(Vector other)
        {
            return Distance(this, other);
        }

        /// <summary>
        /// Finds the distance between the heads of two Vectors
        /// </summary>
        /// <param name="v1">The Vector to discern the angle from </param>
        /// <param name="v2">The Vector to discern the angle to</param>
        /// <returns>The angle between two positional Vectors</returns>
        /// <implementation>
        /// </implementation>
        /// <Acknowledgement>F.Hill, 2001, Computer Graphics using OpenGL, 2ed </Acknowledgement>
        public static Angle Angle(Vector v1, Vector v2)
        {
            return new Angle( System.Math.Acos( Normalize(v1).DotProduct(Normalize(v2)) ) );
        }

        /// <summary>
        /// FindPlugins the angle between this Vector and another
        /// </summary>
        /// <param name="other">The Vector to discern the angle to</param>
        /// <returns>The angle between two positional Vectors</returns>
        /// <implementation>
        /// <see cref="Angle(Vector, Vector)"/>
        /// Uses the Angle(Vector,Vector) method to avoid code duplication
        /// </implementation>
        public Angle Angle(Vector other)
        {
            return Angle(this, other);
        }

        /// <summary>
        /// compares the magnitude of two Vectors and returns the greater Vector
        /// </summary>
        /// <param name="v1">The vector to compare</param>
        /// <param name="v2">The vector to compare with</param>
        /// <returns>
        /// The greater of the two Vectors (based on magnitude)
        /// </returns>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static Vector Max(Vector v1, Vector v2)
        {
            if (v1 >= v2) { return v1; }
            return v2;
        }

        /// <summary>
        /// compares the magnitude of two Vectors and returns the greater Vector
        /// </summary>
        /// <param name="other">The vector to compare with</param>
        /// <returns>
        /// The greater of the two Vectors (based on magnitude)
        /// </returns>
        /// <implementation>
        /// <see cref="Max(Vector, Vector)"/>
        /// Uses function Max(Vector, Vector) to avoid code duplication
        /// </implementation>
        public Vector Max(Vector other)
        {
            return Max(this, other);
        }

        /// <summary>
        /// compares the magnitude of two Vectors and returns the lesser Vector
        /// </summary>
        /// <param name="v1">The vector to compare</param>
        /// <param name="v2">The vector to compare with</param>
        /// <returns>
        /// The lesser of the two Vectors (based on magnitude)
        /// </returns>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static Vector Min(Vector v1, Vector v2)
        {
            if (v1 <= v2) { return v1; }
            return v2;
        }

        /// <summary>
        /// Compares the magnitude of two Vectors and returns the greater Vector
        /// </summary>
        /// <param name="other">The vector to compare with</param>
        /// <returns>
        /// The lesser of the two Vectors (based on magnitude)
        /// </returns>
        /// <implementation>
        /// <see cref="Min(Vector, Vector)"/>
        /// Uses function Min(Vector, Vector) to avoid code duplication
        /// </implementation>
        public Vector Min(Vector other)
        {
            return Min(this, other);
        }

        /// <summary>
        /// FindPlugins the absolute value of a Vector
        /// FindPlugins the magnitude of a Vector
        /// </summary>
        /// <returns>A Vector representing the absolute values of the vector</returns>
        /// <implementation>
        /// An alternative interface to the magnitude property
        /// </implementation>
        public static Double Abs(Vector v1)
        {
            return v1.Magnitude;
        }

        /// <summary>
        /// FindPlugins the absolute value of a Vector
        /// FindPlugins the magnitude of a Vector
        /// </summary>
        /// <returns>A Vector representing the absolute values of the vector</returns>
        /// <implementation>
        /// An alternative interface to the magnitude property
        /// </implementation>
        public double Abs()
        {
            return Magnitude;
        }

        #endregion

        #region Rotation

        public static Vector Rotate(Vector v1, Angle angle, bool x, bool y, bool z)
        {
            if (x) v1 = RotateX(v1, angle);
            if (y) v1 = RotateY(v1, angle);
            if (z) v1 = RotateZ(v1, angle);
            return v1;
        }

        public Vector Rotate(Angle angle, bool x, bool y, bool z)
        {
            return Rotate(this, angle, x, y, z);
        }

        public static Vector RotateX(Vector v1, Angle angle)
        {
            double x = v1.X;
            double y = ( v1.Y * angle.Cos() ) - ( v1.Z * angle.Sin() );
            double z = ( v1.Y * angle.Sin() ) + ( v1.Z * angle.Cos() );
            return new Vector(x, y, z);
        }

        public Vector RotateX(Angle angle)
        {
            return RotateX(this, angle);
        }

        public static Vector RotateY(Vector v1, Angle angle)
        {
            double x = ( v1.Z * angle.Sin() ) + ( v1.X * angle.Cos() );
            double y = v1.Y;
            double z = ( v1.Z * angle.Cos() ) - ( v1.X * angle.Sin() );
            return new Vector(x, y, z);
        }

        public Vector RotateY(Angle angle)
        {
            return RotateY(this, angle);
        }

        public static Vector RotateZ(Vector v1, Angle angle)
        {
            double x = ( v1.X * angle.Cos() ) - ( v1.Y * angle.Sin() );
            double y = ( v1.X * angle.Sin() ) + ( v1.Y * angle.Cos() );
            double z = v1.Z;
            return new Vector(x, y, z);
        }

        public Vector RotateZ(Angle angle)
        {
            return RotateZ(this, angle);
        }

        #endregion

        #region Arbitrary rotation

        public static Vector RotateX(Vector v1, double yOff, double zOff, Angle angle)
        {
            double x = v1.X;
            double y = (v1.Y * angle.Cos()) - (v1.Z * angle.Sin()) + (yOff*(1-angle.Cos()) + zOff*angle.Sin()) ;
            double z = (v1.Y * angle.Sin()) + (v1.Z * angle.Cos()) + (zOff*(1-angle.Cos()) - yOff*angle.Sin()) ;
            return new Vector(x, y, z);
        }

        public Vector RotateX(double yOff, double zOff, Angle angle)
        {
            return RotateX(this, yOff, zOff, angle);
        }

        public static Vector RotateY(Vector v1, double xOff, double zOff, Angle angle)
        {
            double x = (v1.Z * angle.Sin()) + (v1.X * angle.Cos()) + (xOff * (1 - angle.Cos()) - zOff * angle.Sin());
            double y = v1.Y;
            double z = (v1.Z * angle.Cos()) - (v1.X * angle.Sin()) + (zOff * (1 - angle.Cos()) + xOff * angle.Sin());
            return new Vector(x, y, z);
        }

        public Vector RotateY(double xOff, double zOff, Angle angle)
        {
            return RotateY(this, xOff, zOff, angle);
        }

        public static Vector RotateZ(Vector v1, double xOff, double yOff, Angle angle)
        {
            double x = (v1.X * angle.Cos()) - (v1.Y * angle.Sin()) + (xOff * (1 - angle.Cos()) + yOff * angle.Sin());
            double y = (v1.X * angle.Sin()) + (v1.Y * angle.Cos()) + (yOff * (1 - angle.Cos()) - xOff * angle.Sin());
            double z = v1.Z;
            return new Vector(x, y, z);
        }

        public Vector RotateZ(double xOff, double yOff, Angle angle)
        {
            return RotateZ(this, xOff, yOff, angle);
        }

        #endregion

        #region Axis based functions

        /// <summary>
        /// Rotates a Vector around the vertical axis 
        /// Change the yaw of a Vector
        /// </summary>
        /// <param name="v1">The Vector to be rotated</param>
        /// <param name="angle">The angle to rotate the Vector around</param>
        /// <param name="axis">The axis alignments to yaw around</param>
        /// <returns>Rotated vector</returns>
        /// <remarks>
        /// This function is not as efficient as 
        /// <see cref="Rotate(Vector, double, bool, bool, bool)"/>
        /// Rotate(Vector, double, bool, bool, bool)
        /// </remarks>
        public static Vector Yaw(Vector v1, Angle angle, Axis axis)
        {
            bool b;
            double h, v, d; 
            axis.Map(v1.X, v1.Y, v1.Z, out h, out v, out d, out b, out b, out b);

            double h2 = ( d * angle.Sin() ) + ( h * angle.Cos() );
            // v = v;
            double d2 = ( d * angle.Cos() ) - ( h * angle.Sin() );

            double x, y, z;
            axis.Map(out x, out y, out z, h2, v, d2, out b, out b, out b);
            return new Vector(x, y, z);
        }

        /// <summary>
        /// Rotates the Vector around the vertical axis 
        /// Change the yaw of the Vector
        /// </summary>
        /// <param name="angle">The angle to rotate the Vector around</param>
        /// <param name="axis">The axis alignments to yaw around</param>
        /// <returns>Rotated vector</returns>
        /// <implementation>
        /// <see cref="Yaw(Vector, Angle)"/>
        /// Uses function Yaw(Vector, Angle) to avoid code duplication
        /// </implementation>
        /// <remarks>
        /// This function is not as efficient as 
        /// <see cref="Rotate(Vector, double, bool, bool, bool)"/>
        /// Rotate(Vector, double, bool, bool, bool)
        /// </remarks>
        public Vector Yaw(Angle angle, Axis axis)
        {
            return Yaw(this, angle, axis);
        }

        /// <summary>
        /// Rotates a Vector around the horizontal axis
        /// Change the pitch of a Vector
        /// </summary>
        /// <param name="v1">The Vector to be rotated</param>
        /// <param name="angle">The angle to rotate the Vector around</param>
        /// <param name="axis">The axis alignments to pitch around</param>
        /// <returns>Rotated vector</returns>
        /// <remarks>
        /// This function is not as efficient as 
        /// <see cref="Rotate(Vector, double, bool, bool, bool)"/>
        /// Rotate(Vector, double, bool, bool, bool)
        /// </remarks>
        public static Vector Pitch(Vector v1, Angle angle, Axis axis)
        {
            bool b;
            double h, v, d;
            axis.Map(v1.X, v1.Y, v1.Z, out h, out v, out d, out b, out b, out b);

            // h = h;
            double v2 = ( v * angle.Cos() ) - ( d * angle.Sin() );
            double d2 = ( v * angle.Sin() ) + ( d * angle.Cos() );

            double x, y, z;
            axis.Map(out x, out y, out z, h, v2, d2, out b, out b, out b);
            return new Vector(x, y, z);
        }

        /// <summary>
        /// Rotates a Vector around the horizontal axis
        /// Change the pitch of a Vector
        /// </summary>
        /// <param name="angle">The angle to rotate the Vector around</param>
        /// <param name="axis">The axis alignments to pitch around</param>
        /// <returns>Rotated vector</returns>
        /// <see cref="Pitch(Vector, Double)"/>
        /// <implementation>
        /// Uses function Pitch(Vector, Angle) to avoid code duplication
        /// </implementation>
        /// <remarks>
        /// This function is not as efficient as 
        /// <see cref="Rotate(Vector, double, bool, bool, bool)"/>
        /// Rotate(Vector, double, bool, bool, bool)
        /// </remarks>
        public Vector Pitch(Angle angle, Axis axis)
        {
            return Pitch(this, angle, axis);
        }

        /// <summary>
        /// Rotates a Vector around the depth axis
        /// Change the roll of a Vector
        /// </summary>
        /// <param name="v1">The Vector to be rotated</param>
        /// <param name="angle">The angle to rotate the Vector around</param>
        /// <param name="axis">The axis alignments to roll around</param>
        /// <returns>Rotated vector</returns>
        /// <remarks>
        /// This function is not as efficient as 
        /// <see cref="Rotate(Vector, double, bool, bool, bool)"/>
        /// Rotate(Vector, double, bool, bool, bool)
        /// </remarks>
        public static Vector Roll(Vector v1, Angle angle, Axis axis)
        {
            bool b;
            double h, v, d;
            axis.Map(v1.X, v1.Y, v1.Z, out h, out v, out d, out b, out b, out b);

            double h2 = ( h * angle.Cos() ) - ( v * angle.Sin() );
            double v2 = ( h * angle.Sin() ) + ( v * angle.Cos() );
            //d = d;

            double x, y, z;
            axis.Map(out x, out y, out z, h2, v2, d, out b, out b, out b);
            return new Vector(x, y, z);
        }

        /// <summary>
        /// Rotates a Vector around the depth axis
        /// Change the roll of a Vector
        /// </summary>
        /// <param name="angle">The angle to rotate the Vector around</param>
        /// <param name="axis">The axis alignments to roll around</param>
        /// <returns>Rotated vector</returns>
        /// <implementation>
        /// <see cref="Roll(Vector, Angle)"/>
        /// Uses function Roll(Vector, Angle) to avoid code duplication
        /// </implementation>
        /// <remarks>
        /// This function is not as efficient as 
        /// <see cref="Rotate(Vector, double, bool, bool, bool)"/>
        /// Rotate(Vector, double, bool, bool, bool)
        /// </remarks>
        public Vector Roll(Angle angle, Axis axis)
        {
            return Roll(this, angle, axis);
        }

        #endregion

        #region Component Operations

        /// <summary>
        /// The sum of a Vector's components
        /// </summary>
        /// <param name="v1">The vector whose scalar components to sum</param>
        /// <returns>The sum of the Vectors X, Y and Z components</returns>
        public static double SumComponents(Vector v1)
        {
            return (v1.X + v1.Y + v1.Z);
        }

        public static Vector Round(Vector v1)
        {
            return new Vector(System.Math.Round(v1.X), System.Math.Round(v1.Y), System.Math.Round(v1.Z));
        }

        public static Vector Round(Vector v1, int digits)
        {
            return new Vector(System.Math.Round(v1.X, digits), System.Math.Round(v1.Y, digits), System.Math.Round(v1.Z, digits));
        }

        public static Vector Round(Vector v1, MidpointRounding mode)
        {
            return new Vector(System.Math.Round(v1.X, mode), System.Math.Round(v1.Y, mode), System.Math.Round(v1.Z, mode));
        }

        public static Vector Round(Vector v1, int digits, MidpointRounding mode)
        {
            return new Vector(System.Math.Round(v1.X, digits, mode), System.Math.Round(v1.Y, digits, mode), System.Math.Round(v1.Z, digits, mode));
        }

        public Vector Round()
        {
            return new Vector(System.Math.Round(X), System.Math.Round(Y), System.Math.Round(Z));
        }

        public Vector Round(int digits)
        {
            return new Vector(System.Math.Round(X, digits), System.Math.Round(Y, digits), System.Math.Round(Z, digits));
        }

        public Vector Round(MidpointRounding mode)
        {
            return new Vector(System.Math.Round(X, mode), System.Math.Round(Y, mode), System.Math.Round(Z, mode));
        }

        public Vector Round(int digits, MidpointRounding mode)
        {
            return new Vector(System.Math.Round(X, digits, mode), System.Math.Round(Y, digits, mode), System.Math.Round(Z, digits, mode));
        }

        /// <summary>
        /// The sum of this Vector's components
        /// </summary>
        /// <returns>The sum of the Vectors X, Y and Z components</returns>
        /// <implementation>
        /// <see cref="SumComponents(Vector)"/>
        /// The Components.SumComponents(Vector) function has been used to prevent code duplication
        /// </implementation>
        public double SumComponents()
        {
            return SumComponents(this);
        }

        /// <summary>
        /// The sum of a Vector's squared components
        /// </summary>
        /// <param name="v1">The vector whose scalar components to square and sum</param>
        /// <returns>The sum of the Vectors X^2, Y^2 and Z^2 components</returns>
        public static double SumComponentSqrs(Vector v1)
        {
            Vector v2 = SqrComponents(v1);
            return v2.SumComponents();
        }

        /// <summary>
        /// The sum of this Vector's squared components
        /// </summary>
        /// <returns>The sum of the Vectors X^2, Y^2 and Z^2 components</returns>
        /// <implementation>
        /// <see cref="SumComponentSqrs(Vector)"/>
        /// The Components.SumComponentSqrs(Vector) function has been used to prevent code duplication
        /// </implementation>
        public double SumComponentSqrs()
        {
            return SumComponentSqrs(this);
        }

        /// <summary>
        /// The individual multiplication to a power of a Vector's components
        /// </summary>
        /// <param name="v1">The vector whose scalar components to multiply by a power</param>
        /// <param name="power">The power by which to multiply the components</param>
        /// <returns>The multiplied Vector</returns>
        public static Vector PowComponents(Vector v1, double power)
        {
            return
            (
                new Vector
                    (
                        System.Math.Pow(v1.X, power),
                        System.Math.Pow(v1.Y, power),
                        System.Math.Pow(v1.Z, power)
                    )
            );
        }

        /// <summary>
        /// The individual multiplication to a power of this Vector's components
        /// </summary>
        /// <param name="power">The power by which to multiply the components</param>
        /// <returns>The multiplied Vector</returns>
        /// <implementation>
        /// <see cref="PowComponents(Vector, Double)"/>
        /// The Components.PowComponents(Vector, double) function has been used to prevent code duplication
        /// </implementation>
        public Vector PowComponents(double power)
        {
            return PowComponents(this, power);
        }

        /// <summary>
        /// The individual square root of a Vector's components
        /// </summary>
        /// <param name="v1">The vector whose scalar components to square root</param>
        /// <returns>The rooted Vector</returns>
        public static Vector SqrtComponents(Vector v1)
        {
            return
                (
                new Vector
                    (
                        System.Math.Sqrt(v1.X),
                        System.Math.Sqrt(v1.Y),
                        System.Math.Sqrt(v1.Z)
                    )
                );
        }

        /// <summary>
        /// The individual square root of this Vector's components
        /// </summary>
        /// <returns>The rooted Vector</returns>
        /// <implementation>
        /// <see cref="SqrtComponents(Vector)"/>
        /// The Components.SqrtComponents(Vector) function has been used to prevent code duplication
        /// </implementation>
        public Vector SqrtComponents()
        {
            return SqrtComponents(this);
        }

        /// <summary>
        /// The Vector's components squared
        /// </summary>
        /// <param name="v1">The vector whose scalar components are to square</param>
        /// <returns>The squared Vector</returns>
        public static Vector SqrComponents(Vector v1)
        {
            return
                (
                new Vector
                    (
                        v1.X * v1.X,
                        v1.Y * v1.Y,
                        v1.Z * v1.Z
                    )
                );
        }

        /// <summary>
        /// The Vector's components squared
        /// </summary>
        /// <returns>The squared Vector</returns>
        /// <implementation>
        /// <see cref="SqrtComponents(Vector)"/>
        /// The Components.SqrComponents(Vector) function has been used to prevent code duplication
        /// </implementation>
        public Vector SqrComponents()
        {
            return SqrComponents(this);
        }

        #endregion

        #region Standard Functions

        /// <summary>
        /// Textual description of the Vector
        /// </summary>
        /// <Implementation>
        /// Uses ToString(string, IFormatProvider) to avoid code duplication
        /// </Implementation>
        /// <returns>Text (String) representing the vector</returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Textual description of the Vector
        /// </summary>
        /// <param name="format">Formatting string: '_x','_y','_z','', 'm' (vector and magnitude) or 'v' (verbose) followed by standard numeric format string characters valid for a double precision floating point</param>
        /// <param name="formatProvider">The culture specific fromatting provider</param>
        /// <returns>Text (String) representing the vector</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            // If no format is passed
            if (string.IsNullOrEmpty(format)) return String.Format("({0}, {1}, {2})", X, Y, Z);

            char firstChar = Char.ToLower(format[0]);
            string remainder = null;

            if (format.Length > 1)
                remainder = format.Substring(1);

            switch (firstChar)
            {
                case 'x': return X.ToString(remainder, formatProvider);
                case 'y': return Y.ToString(remainder, formatProvider);
                case 'z': return Z.ToString(remainder, formatProvider);
                case 'v':
                    return
                        string.Format
                        (
                            "{0}( x={1}, y={2}, z={3} ){4}",
                            IsUnitVector() ? UNIT_VECTOR : OTHER_VECTOR,
                            X.ToString(remainder, formatProvider),
                            Y.ToString(remainder, formatProvider),
                            Z.ToString(remainder, formatProvider),
                            MAGNITUDE + Magnitude.ToString(remainder, formatProvider)
                        );
                case 'm':
                    return String.Format
                        (
                            "({0}, {1}, {2}) |{3}|",
                            X.ToString(remainder, formatProvider),
                            Y.ToString(remainder, formatProvider),
                            Z.ToString(remainder, formatProvider),
                            Magnitude.ToString(remainder, formatProvider)
                        );
                default:
                    return String.Format
                        (
                            "({0}, {1}, {2})",
                            X.ToString(format, formatProvider),
                            Y.ToString(format, formatProvider),
                            Z.ToString(format, formatProvider)
                        );
            }
        }

        /// <summary>
        /// Get the hashcode
        /// </summary>
        /// <returns>Hashcode for the object instance</returns>
        /// <implementation>
        /// Required in order to implement comparator operations (i.e. ==, !=)
        /// </implementation>
        /// <Acknowledgement>This code is adapted from CSOpenGL - Lucas Viñas Livschitz </Acknowledgement>
        public override int GetHashCode()
        {
            return
            (
                (int)((X + Y + Z) % Int32.MaxValue)
            );
        }

        /// <summary>
        /// Comparator
        /// </summary>
        /// <param name="other">The other object (which should be a vector) to compare to</param>
        /// <returns>Truth if two vectors are equal within a tolerence</returns>
        /// <implementation>
        /// Checks if the object argument is a Vector object 
        /// Uses the equality operator function to avoid code duplication
        /// Required in order to implement comparator operations (i.e. ==, !=)
        /// </implementation>
        public override bool Equals(object other)
        {
            // Check object other is a Vector object
            if (other is Vector)
            {
                // Convert object to Vector
                Vector otherVector = (Vector)other;

                // Check for equality
                return otherVector == this;
            }
            return false;
        }

        public bool Equals(object other, double tolerance)
        {
            if (other is Vector)
            {
                return
                    System.Math.Abs(X - ((Vector)other).X) <= tolerance &&
                    System.Math.Abs(Y - ((Vector)other).Y) <= tolerance &&
                    System.Math.Abs(Z - ((Vector)other).Z) <= tolerance;
            }
            return false;
        }

        /// <summary>
        /// Comparator
        /// </summary>
        /// <param name="other">The other Vector to compare to</param>
        /// <returns>Truth if two vectors are equal within a tolerence</returns>
        /// <implementation>
        /// Uses the equality operator function to avoid code duplication
        /// </implementation>
        public bool Equals(Vector other)
        {
            return other == this;
        }

        public bool Equals(Vector other, double tolerance)
        {
            return
                System.Math.Abs(X - other.X) <= tolerance &&
                System.Math.Abs(Y - other.Y) <= tolerance &&
                System.Math.Abs(Z - other.Z) <= tolerance;
        }

        /// <summary>
        /// compares the magnitude of this instance against the magnitude of the supplied vector
        /// </summary>
        /// <param name="other">The vector to compare this instance with</param>
        /// <returns>
        /// -1: The magnitude of this instance is less than the others magnitude
        /// 0: The magnitude of this instance equals the magnitude of the other
        /// 1: The magnitude of this instance is greater than the magnitude of the other
        /// </returns>
        /// <implementation>
        /// Implemented to fulfil the IComparable interface
        /// </implementation>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public int CompareTo(Vector other)
        {
            return this < other ? -1 : (this > other ? 1 : 0);
        }

        /// <summary>
        /// compares the magnitude of this instance against the magnitude of the supplied vector
        /// </summary>
        /// <param name="other">The vector to compare this instance with</param>
        /// <returns>
        /// -1: The magnitude of this instance is less than the others magnitude
        /// 0: The magnitude of this instance equals the magnitude of the other
        /// 1: The magnitude of this instance is greater than the magnitude of the other
        /// </returns>
        /// <implementation>
        /// Implemented to fulfil the IComparable interface
        /// </implementation>
        /// <exception cref="ArgumentException">
        /// Throws an exception if the type of object to be compared is not known to this class
        /// </exception>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public int CompareTo(object other)
        {
            if (other is Vector)
                return CompareTo((Vector)other);

            // Error condition: other is not a Vector object
            throw new ArgumentException
                (
                // Error message includes information about the actual type of the argument
                NON_VECTOR_COMPARISON + "\n" + ARGUMENT_TYPE + other.GetType(),
                "other"
                );
        }

        #endregion

        #region Decisions

        /// <summary>
        /// Checks if a vector a unit vector
        /// Checks if the Vector has been normalized
        /// Checks if a vector has a magnitude of 1
        /// </summary>
        /// <param name="v1">
        /// <param name="tolerance"></param>
        /// The vector to be checked for Normalization
        /// </param>
        /// <returns>Truth if the vector is a unit vector</returns>
        /// <implementation>
        /// <see cref="Magnitude"/>	
        /// Uses the Magnitude property in the checkBox to avoid code duplication
        /// Within a tolerence
        /// </implementation>
        public static bool IsUnitVector(Vector v1, double tolerance)
        {
            return System.Math.Abs(v1.Magnitude - 1) <= tolerance;
        }

        public static bool IsUnitVector(Vector v1)
        {
            return IsUnitVector(v1, 0);
        }

        /// <summary>
        /// Checks if the vector a unit vector
        /// Checks if the Vector has been normalized 
        /// Checks if the vector has a magnitude of 1
        /// </summary>
        /// <returns>Truth if this vector is a unit vector</returns>
        /// <implementation>
        /// <see cref="IsUnitVector(Vector)"/>	
        /// Uses the isUnitVector(Vector) property in the checkBox to avoid code duplication
        /// Within a tolerence
        /// </implementation>
        public bool IsUnitVector(double tolerance)
        {
            return IsUnitVector(this, tolerance);
        }

        public bool IsUnitVector()
        {
            return IsUnitVector(this);
        }

        /// <summary>
        /// Checks if a face normal vector represents back face
        /// Checks if a face is visible, given the line of sight
        /// </summary>
        /// <param name="normal">
        /// The vector representing the face normal Vector
        /// </param>
        /// <param name="lineOfSight">
        /// The unit vector representing the direction of sight from a virtual camera
        /// </param>
        /// <returns>Truth if the vector (as a normal) represents a back face</returns>
        /// <implementation>
        /// Uses the DotProduct function in the checkBox to avoid code duplication
        /// </implementation>
        public static bool IsBackFace(Vector normal, Vector lineOfSight)
        {
            return normal.DotProduct(lineOfSight) < 0;
        }

        /// <summary>
        /// Checks if a face normal vector represents back face
        /// Checks if a face is visible, given the line of sight
        /// </summary>
        /// <param name="lineOfSight">
        /// The unit vector representing the direction of sight from a virtual camera
        /// </param>
        /// <returns>Truth if the vector (as a normal) represents a back face</returns>
        /// <implementation>
        /// <see cref="Vector.IsBackFace(Vector, Vector)"/> 
        /// Uses the isBackFace(Vector, Vector) function in the checkBox to avoid code duplication
        /// </implementation>
        public bool IsBackFace(Vector lineOfSight)
        {
            return IsBackFace(this, lineOfSight);
        }

        /// <summary>
        /// Checks if two Vectors are perpendicular
        /// Checks if two Vectors are orthogonal
        /// Checks if one Vector is the normal of the other
        /// </summary>
        /// <param name="v1">
        /// The vector to be checked for orthogonality
        /// </param>
        /// <param name="v2">
        /// The vector to be checked for orthogonality to
        /// </param>
        /// <returns>Truth if the two Vectors are perpendicular</returns>
        /// <implementation>
        /// Uses the DotProduct function in the checkBox to avoid code duplication
        /// </implementation>
        public static bool IsPerpendicular(Vector v1, Vector v2)
        {
            return v1.DotProduct(v2) == 0;
        }

        /// <summary>
        /// Checks if two Vectors are perpendicular
        /// Checks if two Vectors are orthogonal
        /// Checks if one Vector is the Normal of the other
        /// </summary>
        /// <param name="other">
        /// The vector to be checked for orthogonality
        /// </param>
        /// <returns>Truth if the two Vectors are perpendicular</returns>
        /// <implementation>
        /// Uses the isPerpendicualr(Vector, Vector) function in the checkBox to avoid code duplication
        /// </implementation>
        public bool IsPerpendicular(Vector other)
        {
            return IsPerpendicular(this, other);
        }

        #endregion

        #region messages

        /// <summary>
        /// Exception message descriptive text 
        /// Used for a failure for an array argument to have three components when three are needed 
        /// </summary>
        private const string THREE_COMPONENTS = "Array must contain exactly three components , (x,y,z)";

        /// <summary>
        /// Exception message descriptive text 
        /// Used for a divide by zero event caused by the normalization of a vector with magnitude 0 
        /// </summary>
        private const string NORMALIZE_0 = "Can not normalize a vector when it's magnitude is zero";

        /// <summary>
        /// Exception message descriptive text 
        /// Used when interpolation is attempted with a control parameter not between 0 and 1 
        /// </summary>
        private const string INTERPOLATION_RANGE = "Control parameter must be a value between 0 & 1";

        /// <summary>
        /// Exception message descriptive text 
        /// Used when attempting to compare a Vector to an object which is not a type of Vector 
        /// </summary>
        private const string NON_VECTOR_COMPARISON = "Cannot compare a Vector to a non-Vector";

        /// <summary>
        /// Exception message additional information text 
        /// Used when adding type information of the given argument into an error message 
        /// </summary>
        private const string ARGUMENT_TYPE = "The argument provided is a type of ";

        /// <summary>
        /// Exception message additional information text 
        /// Used when adding value information of the given argument into an error message 
        /// </summary>
        private const string ARGUMENT_VALUE = "The argument provided has a value of ";

        ///////////////////////////////////////////////////////////////////////////////

        private const string UNIT_VECTOR = "Unit vector composing of ";

        private const string OTHER_VECTOR = "Vector composing of  ";

        private const string MAGNITUDE = " of magnitude ";

        ///////////////////////////////////////////////////////////////////////////////

        #endregion

        #region Constants and Identities

        /// <summary>
        /// The default tolerence used when comparing two vectors 
        /// </summary>
        public const double DefaultTolerence = Double.Epsilon;

        /// <summary>
        /// The smallest vector possible (based on the double precision floating point structure)
        /// </summary>
        public static readonly Vector MinValue = new Vector(Double.MinValue, Double.MinValue, Double.MinValue);

        /// <summary>
        /// The largest vector possible (based on the double precision floating point structure)
        /// </summary>
        public static readonly Vector MaxValue = new Vector(Double.MaxValue, Double.MaxValue, Double.MaxValue);

        /// <summary>
        /// The smallest positive (non-zero) vector possible (based on the double precision floating point structure)
        /// </summary>
        public static readonly Vector Epsilon = new Vector(Double.Epsilon, Double.Epsilon, Double.Epsilon);

        /// <summary>
        /// Vector representing the Cartesian origin
        /// </summary>
        /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
        public static readonly Vector origin = new Vector(0, 0, 0);

        #endregion
    }
}