using System;

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

	using ELException = org.camunda.bpm.engine.impl.javax.el.ELException;

	public interface TypeConverter
	{
		/// <summary>
		/// Default conversions as from JSR245.
		/// </summary>

		/// <summary>
		/// Convert the given input value to the specified target type. </summary>
		/// <param name="value"> input value </param>
		/// <param name="type"> target type </param>
		/// <returns> conversion result </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T convert(Object value, Class<T> type) throws org.camunda.bpm.engine.impl.javax.el.ELException;
		T convert<T>(object value, Type<T> type);
	}

	public static class TypeConverter_Fields
	{
		public static readonly TypeConverter DEFAULT = new TypeConverterImpl();
	}

}