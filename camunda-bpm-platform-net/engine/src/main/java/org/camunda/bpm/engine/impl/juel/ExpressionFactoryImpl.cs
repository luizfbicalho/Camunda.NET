using System;
using System.Collections.Generic;
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
namespace org.camunda.bpm.engine.impl.juel
{

	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELException = org.camunda.bpm.engine.impl.javax.el.ELException;
	using ExpressionFactory = org.camunda.bpm.engine.impl.javax.el.ExpressionFactory;
	using Feature = org.camunda.bpm.engine.impl.juel.Builder.Feature;


	/// <summary>
	/// Expression factory implementation.
	/// 
	/// This class is also used as an EL "service provider". The <em>JUEL</em> jar file specifies this
	/// class as el expression factory implementation in
	/// <code>META-INF/services/javax.el.ExpressionFactory</code>. Calling
	/// <seealso cref="ExpressionFactory#newInstance()"/> will then return an instance of this class, configured as
	/// described below.
	/// 
	/// If no properties are specified at construction time, properties are read from
	/// <ol>
	/// <li>
	/// If the file <code>JAVA_HOME/lib/el.properties</code> exists and if it contains property
	/// <code>javax.el.ExpressionFactory</code> whose value is the name of this class, these properties
	/// are taken as default properties.</li>
	/// <li>Otherwise, if system property <code>javax.el.ExpressionFactory</code> is set to the name of
	/// this class, the system properties <seealso cref="System#getProperties()"/> are taken as default properties.
	/// </li>
	/// <li>
	/// <code>el.properties</code> on your classpath. These properties override the properties from
	/// <code>JAVA_HOME/lib/el.properties</code> or <seealso cref="System#getProperties()"/>.</li>
	/// </ol>
	/// There are also constructors to explicitly pass in an instance of <seealso cref="Properties"/>.
	/// 
	/// Having this, the following properties are read:
	/// <ul>
	/// <li>
	/// <code>javax.el.cacheSize</code> - cache size (int, default is 1000)</li>
	/// <li>
	/// <code>javax.el.methodInvocations</code> - allow method invocations as in
	/// <code>${foo.bar(baz)}</code> (boolean, default is <code>false</code>).</li>
	/// <li>
	/// <code>javax.el.nullProperties</code> - resolve <code>null</code> properties as in
	/// <code>${foo[null]}</code> (boolean, default is <code>false</code>).</li>
	/// <li>
	/// <code>javax.el.varArgs</code> - support function/method calls using varargs (boolean, default is
	/// <code>false</code>).</li>
	/// </ul>
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class ExpressionFactoryImpl : ExpressionFactory
	{
		/// <summary>
		/// A profile provides a default set of language features that will define the builder's
		/// behavior. A profile can be adjusted using the <code>javax.el.methodInvocations</code>,
		/// <code>javax.el.varArgs</code> and <code>javax.el.nullProperties</code> properties.
		/// 
		/// @since 2.2
		/// </summary>
		public sealed class Profile
		{
			/// <summary>
			/// JEE5: none
			/// </summary>
			public static readonly Profile JEE5 = new Profile("JEE5", InnerEnum.JEE5, java.util.EnumSet.noneOf(typeof(org.camunda.bpm.engine.impl.juel.Builder.Feature)));
			/// <summary>
			/// JEE6: <code>javax.el.methodInvocations</code>, <code>javax.el.varArgs</code>. This is the
			/// default profile.
			/// </summary>
			public static readonly Profile JEE6 = new Profile("JEE6", InnerEnum.JEE6, java.util.EnumSet.of(org.camunda.bpm.engine.impl.juel.Builder.Feature.METHOD_INVOCATIONS, org.camunda.bpm.engine.impl.juel.Builder.Feature.VARARGS));

			private static readonly IList<Profile> valueList = new List<Profile>();

			static Profile()
			{
				valueList.Add(JEE5);
				valueList.Add(JEE6);
			}

			public enum InnerEnum
			{
				JEE5,
				JEE6
			}

			public readonly InnerEnum innerEnumValue;
			private readonly string nameValue;
			private readonly int ordinalValue;
			private static int nextOrdinal = 0;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
			internal readonly java.util.EnumSet<org.camunda.bpm.engine.impl.juel.Builder.Feature> features;

			internal Profile(string name, InnerEnum innerEnum, java.util.EnumSet<org.camunda.bpm.engine.impl.juel.Builder.Feature> features)
			{
				this.features_Renamed = features;

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			internal org.camunda.bpm.engine.impl.juel.Builder.Feature[] features()
			{
				return features_Renamed.toArray(new Feature[features_Renamed.size()]);
			}

			internal bool contains(org.camunda.bpm.engine.impl.juel.Builder.Feature feature)
			{
				return features_Renamed.contains(feature);
			}

			public static IList<Profile> values()
			{
				return valueList;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static Profile valueOf(string name)
			{
				foreach (Profile enumInstance in Profile.valueList)
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		/// <summary>
		/// <code>javax.el.methodInvocations</code>
		/// </summary>
		public const string PROP_METHOD_INVOCATIONS = "javax.el.methodInvocations";

		/// <summary>
		/// <code>javax.el.varArgs</code>
		/// </summary>
		public const string PROP_VAR_ARGS = "javax.el.varArgs";

		/// <summary>
		/// <code>javax.el.nullProperties</code>
		/// </summary>
		public const string PROP_NULL_PROPERTIES = "javax.el.nullProperties";

		/// <summary>
		/// <code>javax.el.cacheSize</code>
		/// </summary>
		public const string PROP_CACHE_SIZE = "javax.el.cacheSize";

		private readonly TreeStore store;
		private readonly TypeConverter converter;

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured from <code>el.properties</code> (see above). The maximum
		/// cache size will be 1000 unless overridden in <code>el.properties</code>. The builder profile
		/// is <seealso cref="Profile#JEE6"/> (features may be overridden in <code>el.properties</code>).
		/// </summary>
		public ExpressionFactoryImpl() : this(Profile.JEE6)
		{
		}

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured from the specified profile and <code>el.properties</code>
		/// (see above). The maximum cache size will be 1000 unless overridden in
		/// <code>el.properties</code>.
		/// </summary>
		/// <param name="profile">
		///            builder profile (features may be overridden in <code>el.properties</code>)
		/// 
		/// @since 2.2 </param>
		public ExpressionFactoryImpl(Profile profile)
		{
			Properties properties = loadProperties("el.properties");
			this.store = createTreeStore(1000, profile, properties);
			this.converter = createTypeConverter(properties);
		}

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured using the specified properties. The maximum cache size will
		/// be 1000 unless overridden by property <code>javax.el.cacheSize</code>. The builder profile is
		/// <seealso cref="Profile#JEE6"/> (features may be overridden in <code>properties</code>).
		/// </summary>
		/// <param name="properties">
		///            used to initialize this factory (may be <code>null</code>) </param>
		public ExpressionFactoryImpl(Properties properties) : this(Profile.JEE6, properties)
		{
		}

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured using the specified profile and properties. The maximum
		/// cache size will be 1000 unless overridden by property <code>javax.el.cacheSize</code>.
		/// </summary>
		/// <param name="profile">
		///            builder profile (individual features may be overridden in properties) </param>
		/// <param name="properties">
		///            used to initialize this factory (may be <code>null</code>)
		/// 
		/// @since 2.2 </param>
		public ExpressionFactoryImpl(Profile profile, Properties properties)
		{
			this.store = createTreeStore(1000, profile, properties);
			this.converter = createTypeConverter(properties);
		}

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured using the specified properties. The maximum cache size will
		/// be 1000 unless overridden by property <code>javax.el.cacheSize</code>. The builder profile is
		/// <seealso cref="Profile#JEE6"/> (individual features may be overridden in <code>properties</code>).
		/// </summary>
		/// <param name="properties">
		///            used to initialize this factory (may be <code>null</code>) </param>
		/// <param name="converter">
		///            custom type converter </param>
		public ExpressionFactoryImpl(Properties properties, TypeConverter converter) : this(Profile.JEE6, properties, converter)
		{
		}

		/// <summary>
		/// Create a new expression factory using the default builder and cache implementations. The
		/// builder and cache are configured using the specified profile and properties. The maximum
		/// cache size will be 1000 unless overridden by property <code>javax.el.cacheSize</code>.
		/// </summary>
		/// <param name="profile">
		///            builder profile (individual features may be overridden in properties) </param>
		/// <param name="properties">
		///            used to initialize this factory (may be <code>null</code>) </param>
		/// <param name="converter">
		///            custom type converter
		/// 
		/// @since 2.2 </param>
		public ExpressionFactoryImpl(Profile profile, Properties properties, TypeConverter converter)
		{
			this.store = createTreeStore(1000, profile, properties);
			this.converter = converter;
		}

		/// <summary>
		/// Create a new expression factory.
		/// </summary>
		/// <param name="store">
		///            the tree store used to parse and cache parse trees. </param>
		public ExpressionFactoryImpl(TreeStore store) : this(store, TypeConverter_Fields.DEFAULT)
		{
		}

		/// <summary>
		/// Create a new expression factory.
		/// </summary>
		/// <param name="store">
		///            the tree store used to parse and cache parse trees. </param>
		/// <param name="converter">
		///            custom type converter </param>
		public ExpressionFactoryImpl(TreeStore store, TypeConverter converter)
		{
			this.store = store;
			this.converter = converter;
		}

		private Properties loadDefaultProperties()
		{
			string home = System.getProperty("java.home");
			string path = home + File.separator + "lib" + File.separator + "el.properties";
			File file = new File(path);
			if (file.exists())
			{
				Properties properties = new Properties();
				Stream input = null;
				try
				{
					properties.load(input = new FileStream(file, FileMode.Open, FileAccess.Read));
				}
				catch (IOException e)
				{
					throw new ELException("Cannot read default EL properties", e);
				}
				finally
				{
					try
					{
						input.Close();
					}
					catch (IOException)
					{
						// ignore...
					}
				}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				if (this.GetType().FullName.Equals(properties.getProperty("javax.el.ExpressionFactory")))
				{
					return properties;
				}
			}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			if (this.GetType().FullName.Equals(System.getProperty("javax.el.ExpressionFactory")))
			{
				return System.Properties;
			}
			return null;
		}

		private Properties loadProperties(string path)
		{
			Properties properties = new Properties(loadDefaultProperties());

			// try to find and load properties
			Stream input = null;
			try
			{
				input = Thread.CurrentThread.ContextClassLoader.getResourceAsStream(path);
			}
			catch (SecurityException)
			{
				input = ClassLoader.getSystemResourceAsStream(path);
			}
			if (input != null)
			{
				try
				{
					properties.load(input);
				}
				catch (IOException e)
				{
					throw new ELException("Cannot read EL properties", e);
				}
				finally
				{
					try
					{
						input.Close();
					}
					catch (IOException)
					{
						// ignore...
					}
				}
			}

			return properties;
		}

		private bool getFeatureProperty(Profile profile, Properties properties, Feature feature, string property)
		{
			return Convert.ToBoolean(properties.getProperty(property, profile.contains(feature).ToString()));
		}

		/// <summary>
		/// Create the factory's tree store. This implementation creates a new tree store using the
		/// default builder and cache implementations. The builder and cache are configured using the
		/// specified properties. The maximum cache size will be as specified unless overridden by
		/// property <code>javax.el.cacheSize</code>.
		/// </summary>
		protected internal virtual TreeStore createTreeStore(int defaultCacheSize, Profile profile, Properties properties)
		{
			// create builder
			TreeBuilder builder = null;
			if (properties == null)
			{
				builder = createTreeBuilder(null, profile.features());
			}
			else
			{
				EnumSet<Builder.Feature> features = EnumSet.noneOf(typeof(Builder.Feature));
				if (getFeatureProperty(profile, properties, Feature.METHOD_INVOCATIONS, PROP_METHOD_INVOCATIONS))
				{
					features.add(Builder.Feature.METHOD_INVOCATIONS);
				}
				if (getFeatureProperty(profile, properties, Feature.VARARGS, PROP_VAR_ARGS))
				{
					features.add(Builder.Feature.VARARGS);
				}
				if (getFeatureProperty(profile, properties, Feature.NULL_PROPERTIES, PROP_NULL_PROPERTIES))
				{
					features.add(Builder.Feature.NULL_PROPERTIES);
				}
				builder = createTreeBuilder(properties, features.toArray(new Builder.Feature[0]));
			}

			// create cache
			int cacheSize = defaultCacheSize;
			if (properties != null && properties.containsKey(PROP_CACHE_SIZE))
			{
				try
				{
					cacheSize = int.Parse(properties.getProperty(PROP_CACHE_SIZE));
				}
				catch (System.FormatException e)
				{
					throw new ELException("Cannot parse EL property " + PROP_CACHE_SIZE, e);
				}
			}
			Cache cache = cacheSize > 0 ? new Cache(cacheSize) : null;

			return new TreeStore(builder, cache);
		}

		/// <summary>
		/// Create the factory's type converter. This implementation takes the
		/// <code>de.odysseus.el.misc.TypeConverter</code> property as the name of a class implementing
		/// the <code>de.odysseus.el.misc.TypeConverter</code> interface. If the property is not set, the
		/// default converter (<code>TypeConverter.DEFAULT</code>) is used.
		/// </summary>
		protected internal virtual TypeConverter createTypeConverter(Properties properties)
		{
			Type clazz = load(typeof(TypeConverter), properties);
			if (clazz == null)
			{
				return TypeConverter_Fields.DEFAULT;
			}
			try
			{
				return typeof(TypeConverter).cast(System.Activator.CreateInstance(clazz));
			}
			catch (Exception e)
			{
				throw new ELException("TypeConverter " + clazz + " could not be instantiated", e);
			}
		}

		/// <summary>
		/// Create the factory's builder. This implementation takes the
		/// <code>de.odysseus.el.tree.TreeBuilder</code> property as a name of a class implementing the
		/// <code>de.odysseus.el.tree.TreeBuilder</code> interface. If the property is not set, a plain
		/// <code>de.odysseus.el.tree.impl.Builder</code> is used. If the configured class is a subclass
		/// of <code>de.odysseus.el.tree.impl.Builder</code> and which provides a constructor taking an
		/// array of <code>Builder.Feature</code>, this constructor will be invoked. Otherwise, the
		/// default constructor will be used.
		/// </summary>
		protected internal virtual TreeBuilder createTreeBuilder(Properties properties, params Feature[] features)
		{
			Type clazz = load(typeof(TreeBuilder), properties);
			if (clazz == null)
			{
				return new Builder(features);
			}
			try
			{
				if (clazz.IsAssignableFrom(typeof(Builder)))
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: Constructor<?> constructor = clazz.getConstructor(org.camunda.bpm.engine.impl.juel.Builder.Feature[].class);
					System.Reflection.ConstructorInfo<object> constructor = clazz.GetConstructor(typeof(Feature[]));
					if (constructor == null)
					{
						if (features == null || features.Length == 0)
						{
							return typeof(TreeBuilder).cast(System.Activator.CreateInstance(clazz));
						}
						else
						{
							throw new ELException("Builder " + clazz + " is missing constructor (can't pass features)");
						}
					}
					else
					{
						return typeof(TreeBuilder).cast(constructor.newInstance((object) features));
					}
				}
				else
				{
					return typeof(TreeBuilder).cast(System.Activator.CreateInstance(clazz));
				}
			}
			catch (Exception e)
			{
				throw new ELException("TreeBuilder " + clazz + " could not be instantiated", e);
			}
		}

		private Type load(Type clazz, Properties properties)
		{
			if (properties != null)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				string className = properties.getProperty(clazz.FullName);
				if (!string.ReferenceEquals(className, null))
				{
					ClassLoader loader;
					try
					{
						loader = Thread.CurrentThread.ContextClassLoader;
					}
					catch (Exception e)
					{
						throw new ELException("Could not get context class loader", e);
					}
					try
					{
						return loader == null ? Type.GetType(className) : loader.loadClass(className);
					}
					catch (ClassNotFoundException e)
					{
						throw new ELException("Class " + className + " not found", e);
					}
					catch (Exception e)
					{
						throw new ELException("Class " + className + " could not be instantiated", e);
					}
				}
			}
			return null;
		}

		public override sealed object coerceToType(object obj, Type targetType)
		{
			return converter.convert(obj, targetType);
		}

		public override sealed ObjectValueExpression createValueExpression(object instance, Type expectedType)
		{
			return new ObjectValueExpression(converter, instance, expectedType);
		}

		public override sealed TreeValueExpression createValueExpression(ELContext context, string expression, Type expectedType)
		{
			return new TreeValueExpression(store, context.FunctionMapper, context.VariableMapper, converter, expression, expectedType);
		}

		public override sealed TreeMethodExpression createMethodExpression(ELContext context, string expression, Type expectedReturnType, Type[] expectedParamTypes)
		{
			return new TreeMethodExpression(store, context.FunctionMapper, context.VariableMapper, converter, expression, expectedReturnType, expectedParamTypes);
		}
	}

}