using System;
using System.Text;
using System.Threading;
using System.IO;

/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.javax.el
{

	/// <summary>
	/// Parses a String into a <seealso cref="ValueExpression"/> or <seealso cref="MethodExpression"/> instance for later
	/// evaluation. Classes that implement the EL expression language expose their functionality via this
	/// abstract class. There is no concrete implementation of this API available in this package.
	/// Technologies such as JavaServer Pages and JavaServer Faces provide access to an implementation
	/// via factory methods. The <seealso cref="createValueExpression(ELContext, string, System.Type)"/> method is used
	/// to parse expressions that evaluate to values (both l-values and r-values are supported). The
	/// <seealso cref="createMethodExpression(ELContext, string, System.Type, Class[])"/> method is used to parse
	/// expressions that evaluate to a reference to a method on an object. Unlike previous incarnations
	/// of this API, there is no way to parse and evaluate an expression in one single step. The
	/// expression needs to first be parsed, and then evaluated. Resolution of model objects is performed
	/// at evaluation time, via the <seealso cref="ELResolver"/> associated with the <seealso cref="ELContext"/> passed to
	/// the ValueExpression or MethodExpression. The ELContext object also provides access to the
	/// <seealso cref="FunctionMapper"/> and <seealso cref="VariableMapper"/> to be used when parsing the expression. EL
	/// function and variable mapping is performed at parse-time, and the results are bound to the
	/// expression. Therefore, the <seealso cref="ELContext"/>, <seealso cref="FunctionMapper"/>, and <seealso cref="VariableMapper"/>
	/// are not stored for future use and do not have to be Serializable. The createValueExpression and
	/// createMethodExpression methods must be thread-safe. That is, multiple threads may call these
	/// methods on the same ExpressionFactory object simultaneously. Implementations should synchronize
	/// access if they depend on transient state. Implementations should not, however, assume that only
	/// one object of each ExpressionFactory type will be instantiated; global caching should therefore
	/// be static. The ExpressionFactory must be able to handle the following types of input for the
	/// expression parameter:
	/// <ul>
	/// <li>Single expressions using the ${} delimiter (e.g. "${employee.lastName}").</li>
	/// <li>Single expressions using the #{} delimiter (e.g. "#{employee.lastName}").</li>
	/// <li>Literal text containing no ${} or #{} delimiters (e.g. "John Doe").</li>
	/// <li>Multiple expressions using the same delimiter (e.g.</li>
	/// "${employee.firstName}${employee.lastName}" or "#{employee.firstName}#{employee.lastName}").
	/// <li>Mixed literal text and expressions using the same delimiter (e.g. "Name:
	/// ${employee.firstName} ${employee.lastName}").</li>
	/// </ul>
	/// The following types of input are illegal and must cause an <seealso cref="ELException"/> to be thrown:
	/// <ul>
	/// <li>Multiple expressions using different delimiters (e.g.
	/// "${employee.firstName}#{employee.lastName}").</li>
	/// <li>Mixed literal text and expressions using different delimiters(e.g. "Name:
	/// ${employee.firstName} #{employee.lastName}").</li>
	/// </ul>
	/// </summary>
	public abstract class ExpressionFactory
	{
		/// <summary>
		/// Creates a new instance of a ExpressionFactory. This method uses the following ordered lookup
		/// procedure to determine the ExpressionFactory implementation class to load:
		/// <ul>
		/// <li>Use the Services API (as detailed in the JAR specification). If a resource with the name
		/// of META-INF/services/javax.el.ExpressionFactory exists, then its first line, if present, is
		/// used as the UTF-8 encoded name of the implementation class.</li>
		/// <li>Use the properties file "lib/el.properties" in the JRE directory. If this file exists and
		/// it is readable by the java.util.Properties.load(InputStream) method, and it contains an entry
		/// whose key is "javax.el.ExpressionFactory", then the value of that entry is used as the name
		/// of the implementation class.</li>
		/// <li>Use the javax.el.ExpressionFactory system property. If a system property with this name
		/// is defined, then its value is used as the name of the implementation class.</li>
		/// <li>Use a platform default implementation.</li>
		/// </ul>
		/// </summary>
		/// <returns> An instance of ExpressionFactory. </returns>
		/// <exception cref="ELException">
		///             if a factory class cannot be found or instantiation fails. </exception>
		public static ExpressionFactory newInstance()
		{
			return newInstance(null);
		}

		/// <summary>
		/// Create a new instance of a ExpressionFactory, with optional properties. This method uses the
		/// same lookup procedure as the one used in newInstance(). If the argument properties is not
		/// null, and if the implementation contains a constructor with a single parameter of type
		/// java.util.Properties, then the constructor is used to create the instance. Properties are
		/// optional and can be ignored by an implementation. The name of a property should start with
		/// "javax.el." The following are some suggested names for properties.
		/// <ul>
		/// <li>javax.el.cacheSize</li>
		/// </ul>
		/// </summary>
		/// <param name="properties">
		///            Properties passed to the constructor of the implementation. </param>
		/// <returns> An instance of ExpressionFactory. </returns>
		/// <exception cref="ELException">
		///             if a factory class cannot be found or instantiation fails. </exception>
		public static ExpressionFactory newInstance(Properties properties)
		{
			ClassLoader classLoader;
			try
			{
				classLoader = Thread.CurrentThread.ContextClassLoader;
			}
			catch (SecurityException)
			{
				classLoader = typeof(ExpressionFactory).ClassLoader;
			}

			string className = null;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string serviceId = "META-INF/services/" + typeof(ExpressionFactory).FullName;
			Stream input = classLoader.getResourceAsStream(serviceId);
			try
			{
				if (input != null)
				{
					StreamReader reader = new StreamReader(input, Encoding.UTF8);
					className = reader.ReadLine();
					reader.Close();
				}
			}
			catch (IOException)
			{
				// do nothing
			}
			finally
			{
				if (input != null)
				{
					try
					{
						input.Close();
					}
					catch (Exception)
					{
						// do nothing
					}
					finally
					{
						input = null;
					}
				}
			}

			if (string.ReferenceEquals(className, null) || className.Trim().Length == 0)
			{
				try
				{
					string home = System.getProperty("java.home");
					if (!string.ReferenceEquals(home, null))
					{
						string path = home + File.separator + "lib" + File.separator + "el.properties";
						File file = new File(path);
						if (file.exists())
						{
							input = new FileStream(file, FileMode.Open, FileAccess.Read);
							Properties props = new Properties();
							props.load(input);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							className = props.getProperty(typeof(ExpressionFactory).FullName);
						}
					}
				}
				catch (IOException)
				{
					// do nothing
				}
				catch (SecurityException)
				{
					// do nothing
				}
				finally
				{
					if (input != null)
					{
						try
						{
							input.Close();
						}
						catch (IOException)
						{
							// do nothing
						}
						finally
						{
							input = null;
						}
					}
				}
			}

			if (string.ReferenceEquals(className, null) || className.Trim().Length == 0)
			{
				try
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					className = System.getProperty(typeof(ExpressionFactory).FullName);
				}
				catch (Exception)
				{
					// do nothing
				}
			}

			if (string.ReferenceEquals(className, null) || className.Trim().Length == 0)
			{
				className = "org.camunda.bpm.engine.impl.juel.ExpressionFactoryImpl";
			}

			return newInstance(properties, className, classLoader);
		}

		/// <summary>
		/// Create an ExpressionFactory instance.
		/// </summary>
		/// <param name="properties">
		///            Properties passed to the constructor of the implementation. </param>
		/// <returns> an instance of ExpressionFactory </returns>
		/// <param name="className">
		///            The name of the ExpressionFactory class. </param>
		/// <param name="classLoader">
		///            The class loader to be used to load the class. </param>
		/// <returns> An instance of ExpressionFactory. </returns>
		/// <exception cref="ELException">
		///             if the class could not be found or if it is not a subclass of ExpressionFactory
		///             or if the class could not be instantiated. </exception>
		private static ExpressionFactory newInstance(Properties properties, string className, ClassLoader classLoader)
		{
			Type clazz = null;
			try
			{
				clazz = classLoader.loadClass(className.Trim());
				if (!clazz.IsAssignableFrom(typeof(ExpressionFactory)))
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new ELException("Invalid expression factory class: " + clazz.FullName);
				}
			}
			catch (ClassNotFoundException e)
			{
				throw new ELException("Could not find expression factory class", e);
			}
			try
			{
				if (properties != null)
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: Constructor<?> constructor = null;
					System.Reflection.ConstructorInfo<object> constructor = null;
					try
					{
						constructor = clazz.GetConstructor(typeof(Properties));
					}
					catch (Exception)
					{
						// do nothing
					}
					if (constructor != null)
					{
						return (ExpressionFactory) constructor.newInstance(properties);
					}
				}
				return (ExpressionFactory) System.Activator.CreateInstance(clazz);
			}
			catch (Exception e)
			{
				throw new ELException("Could not create expression factory instance", e);
			}
		}

		/// <summary>
		/// Coerces an object to a specific type according to the EL type conversion rules. An
		/// <seealso cref="ELException"/> is thrown if an error results from applying the conversion rules.
		/// </summary>
		/// <param name="obj">
		///            The object to coerce. </param>
		/// <param name="targetType">
		///            The target type for the coercion. </param>
		/// <returns> the coerced object </returns>
		/// <exception cref="ELException">
		///             if an error results from applying the conversion rules. </exception>
		public abstract object coerceToType(object obj, Type targetType);

		/// <summary>
		/// Parses an expression into a <seealso cref="MethodExpression"/> for later evaluation. Use this method
		/// for expressions that refer to methods. If the expression is a String literal, a
		/// MethodExpression is created, which when invoked, returns the String literal, coerced to
		/// expectedReturnType. An ELException is thrown if expectedReturnType is void or if the coercion
		/// of the String literal to the expectedReturnType yields an error (see Section "1.16 Type
		/// Conversion"). This method should perform syntactic validation of the expression. If in doing
		/// so it detects errors, it should raise an ELException.
		/// </summary>
		/// <param name="context">
		///            The EL context used to parse the expression. The FunctionMapper and VariableMapper
		///            stored in the ELContext are used to resolve functions and variables found in the
		///            expression. They can be null, in which case functions or variables are not
		///            supported for this expression. The object returned must invoke the same functions
		///            and access the same variable mappings regardless of whether the mappings in the
		///            provided FunctionMapper and VariableMapper instances change between calling
		///            ExpressionFactory.createMethodExpression() and any method on MethodExpression.
		///            Note that within the EL, the ${} and #{} syntaxes are treated identically. This
		///            includes the use of VariableMapper and FunctionMapper at expression creation time.
		///            Each is invoked if not null, independent of whether the #{} or ${} syntax is used
		///            for the expression. </param>
		/// <param name="expression">
		///            The expression to parse </param>
		/// <param name="expectedReturnType">
		///            The expected return type for the method to be found. After evaluating the
		///            expression, the MethodExpression must check that the return type of the actual
		///            method matches this type. Passing in a value of null indicates the caller does not
		///            care what the return type is, and the check is disabled. </param>
		/// <param name="expectedParamTypes">
		///            The expected parameter types for the method to be found. Must be an array with no
		///            elements if there are no parameters expected. It is illegal to pass null. </param>
		/// <returns> The parsed expression </returns>
		/// <exception cref="ELException">
		///             Thrown if there are syntactical errors in the provided expression. </exception>
		/// <exception cref="NullPointerException">
		///             if paramTypes is null. </exception>
		public abstract MethodExpression createMethodExpression(ELContext context, string expression, Type expectedReturnType, Type[] expectedParamTypes);

		/// <summary>
		/// Parses an expression into a <seealso cref="ValueExpression"/> for later evaluation. Use this method for
		/// expressions that refer to values. This method should perform syntactic validation of the
		/// expression. If in doing so it detects errors, it should raise an ELException.
		/// </summary>
		/// <param name="context">
		///            The EL context used to parse the expression. The FunctionMapper and VariableMapper
		///            stored in the ELContext are used to resolve functions and variables found in the
		///            expression. They can be null, in which case functions or variables are not
		///            supported for this expression. The object returned must invoke the same functions
		///            and access the same variable mappings regardless of whether the mappings in the
		///            provided FunctionMapper and VariableMapper instances change between calling
		///            ExpressionFactory.createValueExpression() and any method on ValueExpression. Note
		///            that within the EL, the ${} and #{} syntaxes are treated identically. This
		///            includes the use of VariableMapper and FunctionMapper at expression creation time.
		///            Each is invoked if not null, independent of whether the #{} or ${} syntax is used
		///            for the expression. </param>
		/// <param name="expression">
		///            The expression to parse </param>
		/// <param name="expectedType">
		///            The type the result of the expression will be coerced to after evaluation. </param>
		/// <returns> The parsed expression </returns>
		/// <exception cref="ELException">
		///             Thrown if there are syntactical errors in the provided expression. </exception>
		/// <exception cref="NullPointerException">
		///             if paramTypes is null. </exception>
		public abstract ValueExpression createValueExpression(ELContext context, string expression, Type expectedType);

		/// <summary>
		/// Creates a ValueExpression that wraps an object instance. This method can be used to pass any
		/// object as a ValueExpression. The wrapper ValueExpression is read only, and returns the
		/// wrapped object via its getValue() method, optionally coerced.
		/// </summary>
		/// <param name="instance">
		///            The object instance to be wrapped. </param>
		/// <param name="expectedType">
		///            The type the result of the expression will be coerced to after evaluation. There
		///            will be no coercion if it is Object.class, </param>
		/// <returns> a ValueExpression that wraps the given object instance. </returns>
		public abstract ValueExpression createValueExpression(object instance, Type expectedType);
	}

}